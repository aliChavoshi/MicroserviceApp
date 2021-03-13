using System.Collections.Generic;

namespace Basket.Api.Entities
{
    public class BasketCart
    {
        public BasketCart()
        {
            Items = new List<BasketCartItem>();
        }
        public string UserName { get; set; }

        public List<BasketCartItem> Items { get; set; }

        //calculate total price basket cart
        public decimal TotalPrice()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }
}