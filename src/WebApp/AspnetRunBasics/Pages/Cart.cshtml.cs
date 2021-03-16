using System;
using System.Linq;
using System.Threading.Tasks;
using AspnetRunBasics.ApiCollection.Interfaces;
using AspnetRunBasics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics.Pages
{
    public class CartModel : PageModel
    {
        private readonly IBasketApi _basketApi;

        public CartModel(IBasketApi basketApi)
        {
            _basketApi = basketApi;
        }
        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = await _basketApi.GetBasket(username: "swn");
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            var userName = "swn";
            var basket = await _basketApi.GetBasket(userName);

            var item = basket.Items.SingleOrDefault(x => x.ProductId == productId);
            basket.Items.Remove(item: item);

            var basketUpdated = await _basketApi.UpdateBasket(basket);
            return RedirectToPage();
        }
    }
}