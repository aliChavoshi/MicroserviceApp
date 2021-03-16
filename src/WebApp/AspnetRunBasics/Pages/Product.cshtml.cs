using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class ProductModel : PageModel
    {
        private readonly IBasketApi _basketApi;
        private readonly ICatalogApi _catalogApi;

        public ProductModel(IBasketApi basketApi, ICatalogApi catalogApi)
        {
            _basketApi = basketApi;
            _catalogApi = catalogApi;
        }
        public IEnumerable<string> CategoryList { get; set; } = new List<string>();
        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(string categoryName)
        {
            var productList = (await _catalogApi.GetCatalog()).ToList();
            CategoryList = productList.Select(x => x.Category).Distinct();
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                ProductList = productList.Where(x => x.Category == categoryName);
                SelectedCategory = categoryName;
            }
            else
            {
                ProductList = productList;
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