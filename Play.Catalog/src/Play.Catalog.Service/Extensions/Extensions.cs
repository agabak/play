using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            // you want to have the same instance at ll time
            services.AddSingleton(serviceProvider =>
            {
                var _config = serviceProvider.GetService<IConfiguration>();

                var serviceSettings = _config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongodbSettings = _config.GetSection(nameof(MongodbSettings)).Get<MongodbSettings>();

                var mongoClient = new MongoClient(mongodbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(
            this IServiceCollection services, 
            string collectionName) where T :Entities.IEntity
        {
            services.AddScoped<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}