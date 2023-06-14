using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ApiIntegration
{
    public class OrderApiClient : BaseApiClient, IOrderApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrderApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Create(CheckoutRequest request, Guid? userId)
        {
            var sessions = _httpContextAccessor
        .HttpContext
        .Session
        .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestData = new
            {
                request.Name,
                userId,
                request.PhoneNumber,
                request.Address,
                request.Email,
                OrderDetails = request.OrderDetails
            };

            //var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData));

            //var response = await client.PostAsync("/api/orders/", jsonContent);

            var response = await client.PostAsJsonAsync("/api/orders/", requestData);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderInforVm>> GetAll(string languageId)
        {
            var data = await GetListAsync<OrderInforVm>($"/api/orders?languageId={languageId}");
            return data;
        }

        public Task<OrderInforVm> GetOrderById(int orderId, string languageId)
        {
            var data = GetAsync<OrderInforVm>($"api/orders/{orderId}/{languageId}");
            return data;
        }

        public async Task<bool> UpdateOrderStatus(int id)
        {
            var sessions = _httpContextAccessor
                             .HttpContext
                             .Session
                             .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(id);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"/api/orders/updateOrderStatus/{id}", httpContent);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> CancelOrderStatus(int id)
        {
            var sessions = _httpContextAccessor
                            .HttpContext
                            .Session
                            .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(id);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"/api/orders/cancelOrderStatus/{id}", httpContent);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}