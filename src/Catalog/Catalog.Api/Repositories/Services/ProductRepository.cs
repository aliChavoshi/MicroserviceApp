using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Data.Interfaces;
using Catalog.Api.Entities;
using Catalog.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace Catalog.Api.Repositories.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products
                .FindSync(p => true)
                .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products
                .FindSync(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            var filter = Builders<Product>.Filter.ElemMatch(x => x.Name, name);

            return await _context.Products
                .FindSync(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            var filters = Builders<Product>.Filter.ElemMatch(x=>x.Category.ToLower(),category.ToLower());

            return await _context.Products
                .FindSync(filters)
                .ToListAsync();
        }

        public async Task Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> Update(Product product)
        {
            var updateResult =
                await _context.Products
                    .ReplaceOneAsync(filter: x => x.Id == product.Id,
                                     replacement: product);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            var result = await _context.Products.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}