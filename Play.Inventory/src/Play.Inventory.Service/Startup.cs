using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace Play.Inventory.Service
{
    public class Startup
    {
        private ServiceSettings serviceSettings;
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            serviceSettings = _config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo()
                    .AddMongoRepository<InventoryItem>("InventoryItem");

            Random jitter = new Random();
            services.AddHttpClient<CatalogClient>(client => 
            {
                client.BaseAddress = new Uri("https://localhost:5001");

            }).AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>()
              .WaitAndRetryAsync(5,retryAttemp => TimeSpan.FromSeconds(Math.Pow(2,retryAttemp)) 
                                                + TimeSpan.FromMilliseconds(jitter.Next(0,1000)) 
                                                
            )).AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>()
              .CircuitBreakerAsync(3,TimeSpan.FromSeconds(15)))
              .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

            services.AddControllers(opts => 
            {
                opts.SuppressAsyncSuffixInActionNames = true;   
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Inventory.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}//the circuit breaker pattern
// Prevents the service from performing an operation that's likely to fail
// mass trans help to separate q message service
