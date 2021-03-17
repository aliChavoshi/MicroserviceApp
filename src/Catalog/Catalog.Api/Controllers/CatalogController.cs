using Catalog.Api.Entities;
using Catalog.Api.Repositories.Interfaces;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<Product> _logger;
        public CatalogController(IProductRepository productRepository, ILogger<Product> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> GetProducts()
        {
            var result = await _productRepository.GetProducts();
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] //404
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> GetProductById(string id)
        {
            var result = await _productRepository.GetProduct(id);
            if (result != null) return Ok(result);

            _logger.LogError($"Product with id : {id}, NotFound.");
            return NotFound();
        }

        [Route("[action]/{category}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] //404
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> GetProductByCategoryName(string category)
        {
            var result = await _productRepository.GetProductsByCategory(category);
            if (result != null) return Ok(result);

            _logger.LogError($"Product with category Name : {category}, NotFound.");
            return NotFound();
        }

        [HttpGet("[action]/{name}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] //404
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> GetProductByName(string name)
        {
            var result = await _productRepository.GetProductsByName(name);
            if (result != null) return Ok(result);

            _logger.LogError($"Product with Name : {name}, not found .");
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            await _productRepository.Create(product);
            return RedirectToAction("GetProductById", "Catalog", new { id = product.Id });
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)] //200
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _productRepository.Update(product));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)] //200
        public async Task<IActionResult> DeleteProduct(string id)
        {
            return Ok(await _productRepository.Delete(id));
        }
    }
}
