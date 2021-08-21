using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;
using System.Reflection;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configuration) =>
                {
                    var _config = context.GetService<IConfiguration>();
                    var rabbitMqSettings = _config.GetSection(nameof(RabbitMQSettings))
                                                  .Get<RabbitMQSettings>();
                    var serviceSettings = _config.GetSection(nameof(ServiceSettings))
                                                 .Get<ServiceSettings>();

                        configuration.Host(rabbitMqSettings.Host);

                        configuration.ConfigureEndpoints(context,
                                  new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                });
            });

            services.AddMassTransitHostedService();
            return services;
        }
    }
}
