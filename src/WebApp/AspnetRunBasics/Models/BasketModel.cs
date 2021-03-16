using System.Collections.Generic;

namespace AspnetRunBasics.Models
{
    public class BasketModel
    {
        public BasketModel()
        {
            Items = new List<BasketItemModel>();
        }
        public string UserName { get; set; }

        public List<BasketItemModel> Items { get; set; }

        //calculate total price basket cart
        public decimal TotalPrice { get; set; }
    }
}