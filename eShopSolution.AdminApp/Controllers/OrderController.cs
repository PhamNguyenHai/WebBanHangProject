using eShopSolution.ApiIntegration;
using eShopSolution.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace eShopSolution.AdminApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderApiClient _orderApiClient;
        private readonly IConfiguration _configuration;

        public OrderController(IOrderApiClient orderApiClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _orderApiClient = orderApiClient;
        }

        public IActionResult Index()
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var orderList = _orderApiClient.GetAll(languageId).Result;
            return View(orderList);
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var order = _orderApiClient.GetOrderById(Id, languageId).Result;
            return View(order);
        }
    }
}