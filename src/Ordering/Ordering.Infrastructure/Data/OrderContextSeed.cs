using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context,
                     ILoggerFactory loggerFactory, int? retry = 0)
        {
            var retryForAvailability = retry.GetValueOrDefault(0);
            try
            {
                await context.Database.MigrateAsync();
                if (!await context.Order.AnyAsync())
                {
                    await context.Order.AddRangeAsync(GetPerConfigOrders());
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                if (retryForAvailability < 3)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(exception.Message);

                    await SeedAsync(context, loggerFactory, retryForAvailability);
                }
            }
        }

        private static IEnumerable<Order> GetPerConfigOrders()
        {
            return new List<Order>
            {
                new()
                {
                    AddressLine = "kashan",
                    UserName = "Ali",
                    FirstName = "Ali",
                    LastName = "Chavoshi",
                    EmailAddress = "Alichavoshii1372@gamil.com",
                    Country = "Iran",
                    CVV = "adf",
                    CardName = "asef",
                    CardNumber = "saef",
                    Expiration = "sadf",
                    PaymentMethod = 6,
                    State = "sefjn",
                    TotalPrice = 541,
                    ZipCode = "sdf"
                }
            };
        }
    }
}