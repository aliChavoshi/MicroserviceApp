using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Api.RabbitMQ;

namespace Ordering.Api.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static EventBusRabbitMqConsumer Listener { get; set; }
        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<EventBusRabbitMqConsumer>();
            var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            if (life == null) return app;

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopped);

            return app;
        }

        private static void OnStarted()
        {
            Listener.Consume();
        }
        private static void OnStopped()
        {
            Listener.Disconnect();
        }

    }
}