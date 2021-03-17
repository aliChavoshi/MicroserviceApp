using System;
using System.Threading.Tasks;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IBasketApi _basketApi;
        private readonly ICatalogApi _catalogApi;

        public ProductDetailModel(IBasketApi basketApi, ICatalogApi catalogApi)
        {
            _basketApi = basketApi;
            _catalogApi = catalogApi;
        }

        public CatalogModel Product { get; set; } = new CatalogModel();

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public async Task<IActionResult> OnGetAsync(string productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Product = await _catalogApi.GetCatalog(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogApi.GetCatalog(productId);
            var userName = "swn";
            var basket = await _basketApi.GetBasket(userName);
            basket.Items.Add(new BasketItemModel()
            {
                Color = "Red",
                Price = product.Price,
                ProductId = productId,
                ProductName = product.Name,
                Quantity = 1
            });

            var basketUpdated = await _basketApi.UpdateBasket(basket);
            return RedirectToPage("Cart");
        }
    }
}