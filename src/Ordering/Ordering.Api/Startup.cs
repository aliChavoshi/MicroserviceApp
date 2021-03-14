using System.Reflection;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Producer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.Api.Extensions;
using Ordering.Api.Mapping;
using Ordering.Api.RabbitMQ;
using Ordering.Application.Handlers;
using Ordering.Application.Mapper;
using Ordering.Core.Repositories;
using Ordering.Core.Repositories.Base;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Repositories.Base;
using RabbitMQ.Client;

namespace Ordering.Api
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
            services.AddControllers();

            //Connection String
            services.AddDbContextPool<OrderContext>(c =>
                c.UseSqlServer(Configuration.GetConnectionString("OrderConnection")));

            //IOC
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            services.AddTransient<IOrderRepository, OrderRepository>();

            //Auto Mapper
            services.AddAutoMapper(typeof(Startup));

            //CQRS
            services.AddMediatR(typeof(CheckoutOrderHandler).GetTypeInfo().Assembly);

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Order API",
                    Version = "v1",
                    Contact = new OpenApiContact() { Email = "Alichavoshii1372@gamil.com", Name = "AliChavoshi" },
                    Description = "This is Order Api"
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
            //Consumer
            services.AddScoped<EventBusRabbitMqConsumer>();
            #endregion
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
            });

            app.UseRouting();

            app.UseAuthorization();

            //Listener RabbitMQ
            app.UseRabbitListener();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
