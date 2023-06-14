using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntegration
{
    public interface IOrderApiClient
    {
        Task<bool> Create(CheckoutRequest request, Guid? userId);

        Task<List<OrderInforVm>> GetAll(string languageId);

        Task<OrderInforVm> GetOrderById(int orderId, string languageId);
    }
}