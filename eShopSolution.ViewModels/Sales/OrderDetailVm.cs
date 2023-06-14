using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Sales
{
    public class OrderDetailVm
    {
        public string ProductName { get; set; }

        public string PathImg { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }
    }
}