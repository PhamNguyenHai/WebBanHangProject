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
        [Authorize]
        public async Task<IActionResult> GetAllOrder(string languageId)
        {
            var result = await _orderService.GetAll(languageId);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
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
        [Authorize]
        public async Task<IActionResult> GetById(int orderId, string languageId)
        {
            var order = await _orderService.GetOrderById(orderId, languageId);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPatch("updateOrderStatus/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(int id)
        {
            var result = await _orderService.UpdateOrderStatus(id);
            if (result.IsSuccessed)
                return Ok();
            return BadRequest("Không huỷ được đơn hàng");
        }

        [HttpPatch("cancelOrderStatus/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrderStatus(int id)
        {
            var result = await _orderService.CancelOrderStatus(id);
            if (result.IsSuccessed)
                return Ok();
            return BadRequest("Không huỷ được đơn hàng");
        }
    }
}