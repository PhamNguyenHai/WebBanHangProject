using eShopSolution.ViewModels.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Sales
{
    public class OrderInforVm
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime OrderDate { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public OrderStatus Status { set; get; }

        public List<OrderDetailVm> OrderDetails { set; get; } = new List<OrderDetailVm>();
    }
}