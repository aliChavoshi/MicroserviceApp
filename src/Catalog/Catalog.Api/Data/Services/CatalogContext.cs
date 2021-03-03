using Catalog.Api.Data.Interfaces;
using Catalog.Api.Entities;
using Catalog.Api.Settings;
using DnsClient.Protocol;
using MongoDB.Driver;

namespace Catalog.Api.Data.Services
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(ICatalogDatabaseSettings settings)
        {
            var client = new MongoClient(connectionString: settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            Products = database.GetCollection<Product>(settings.CollectionName);

            //Seed Data
            CatalogContextSeedData.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}