using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Orders;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder(string languageId)
        {
            var result = await _orderService.GetAll(languageId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderId = await _orderService.Create(request, null);
            if (orderId == 0)
                return BadRequest("Fail to create order");

            var order = await _orderService.GetOrderById(orderId, "vi");
            return CreatedAtAction(nameof(GetById), new { id = orderId }, order);
        }

        [HttpGet("{orderId}/{languageId}")]
        public async Task<IActionResult> GetById(int orderId, string languageId)
        {
            var order = await _orderService.GetOrderById(orderId, languageId);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
    }
}