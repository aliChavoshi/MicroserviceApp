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
                new Order() {UserName = "swn", FirstName = "Mehmet", LastName = "Ozkaya", EmailAddress = "ezozkme@gmail.com", AddressLine = "Bahcelievler", Country = "Turkey", TotalPrice = 350 }
            };
        }
    }
}