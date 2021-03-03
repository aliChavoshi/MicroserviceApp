using Basket.Api.Data.Interfaces;
using Basket.Api.Data.Services;
using Basket.Api.Repositories.Interfaces;
using Basket.Api.Repositories.Services;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Basket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions
                    .Parse(Configuration.GetConnectionString("Redis"), true);

                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddControllers();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketContext, BasketContext>();
            services.AddSingleton<EventBusRabbitMqProducer>();

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Basket API",
                    Version = "v1",
                    Contact = new OpenApiContact() { Email = "Alichavoshii1372@gamil.com", Name = "AliChavoshi" },
                    Description = "This is Basket Api"
                });
            });

            #endregion

            #region RabbitMQ

            services.AddSingleton<IRabbitMqConnection>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBus:HostName"]
                };

                var username = Configuration["EventBus:UserName"];
                var password = Configuration["EventBus:Password"];

                if (!string.IsNullOrEmpty(username))
                    factory.UserName = username;
                if (!string.IsNullOrEmpty(password))
                    factory.Password = password;

                return new RabbitMqConnection(factory);

            });

            #endregion

            services.AddAutoMapper(typeof(Mapping.Mapping));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
