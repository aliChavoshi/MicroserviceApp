using System.Collections.Generic;
using Catalog.Api.Entities;
using MongoDB.Driver;

namespace Catalog.Api.Data
{
    public class CatalogContextSeedData
    {
        public static void SeedData(IMongoCollection<Product> collection)
        {
            var existProducts = collection.Find(x => true).Any();
            if (!existProducts)
                collection.InsertMany(GenerateProducts());
        }

        private static IEnumerable<Product> GenerateProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Name = "IPhone x",
                    Summary = "This Phone Is Best IPhone",
                    Category = "Smart Phone",
                    Description = "",
                    ImageFile = "1.png",
                    Price = 950.00M
                },
                new Product()
                {
                    Name = "Note 21 ",
                    Summary = "This Phone Is Best Sumsung",
                    Category = "Smart Phone",
                    Description = "",
                    ImageFile = "2.png",
                    Price = 10000M
                }
            };
        }
    }
}