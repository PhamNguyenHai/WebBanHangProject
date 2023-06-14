using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eShopSolution.ViewModels.Utilities.Enums;
using eShopSolution.ViewModels.Common;

namespace eShopSolution.Application.Catalog.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EShopDbContext _context;

        public OrderService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> CancelOrderStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            order.Status = (Data.Enums.OrderStatus)4;

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<int> Create(CheckoutRequest request, Guid? userId)
        {
            Order newOrder = new Order()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                ShipName = request.Name,
                ShipAddress = request.Address,
                ShipEmail = request.Email,
                ShipPhoneNumber = request.PhoneNumber,
                Status = 0
            };

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var orderDt in request.OrderDetails)
            {
                var Pro = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDt.ProductId);
                orderDetails.Add(new OrderDetail()
                {
                    OrderId = newOrder.Id,
                    ProductId = orderDt.ProductId,
                    Quantity = orderDt.Quantity,
                    Price = Pro.Price * orderDt.Quantity,
                });
                Pro.Stock -= orderDt.Quantity;
            }

            newOrder.OrderDetails = orderDetails;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            return newOrder.Id;
        }

        public async Task<List<OrderInforVm>> GetAll(string languageId)
        {
            var query = from o in _context.Orders
                        select new OrderInforVm()
                        {
                            Id = o.Id,
                            Name = o.ShipName,
                            OrderDate = o.OrderDate,
                            Address = o.ShipAddress,
                            Email = o.ShipEmail,
                            Status = (OrderStatus)o.Status,
                            PhoneNumber = o.ShipPhoneNumber,
                            OrderDetails = (from od in _context.OrderDetails
                                            join p in _context.Products on od.ProductId equals p.Id
                                            join pi in _context.ProductImages on p.Id equals pi.ProductId
                                            join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                                            where languageId == pt.LanguageId && o.Id == od.OrderId
                                            select new OrderDetailVm()
                                            {
                                                ProductName = pt.Name,
                                                PathImg = pi.ImagePath,
                                                Price = p.Price,
                                                Quantity = od.Quantity,
                                                Total = p.Price * od.Quantity
                                            }).ToList()
                        };
            return await query.ToListAsync();
        }

        public async Task<OrderInforVm> GetOrderById(int orderId, string languageId)
        {
            var query = from o in _context.Orders
                        where o.Id == orderId
                        select new OrderInforVm()
                        {
                            Id = o.Id,
                            Name = o.ShipName,
                            OrderDate = o.OrderDate,
                            Address = o.ShipAddress,
                            Email = o.ShipEmail,
                            Status = (OrderStatus)o.Status,
                            PhoneNumber = o.ShipPhoneNumber,
                            OrderDetails = (from od in _context.OrderDetails
                                            join p in _context.Products on od.ProductId equals p.Id
                                            join pi in _context.ProductImages on p.Id equals pi.ProductId
                                            join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                                            where languageId == pt.LanguageId && o.Id == od.OrderId
                                            select new OrderDetailVm()
                                            {
                                                ProductName = pt.Name,
                                                PathImg = pi.ImagePath,
                                                Price = p.Price,
                                                Quantity = od.Quantity,
                                                Total = p.Price * od.Quantity
                                            }).ToList()
                        };
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ApiResult<bool>> UpdateOrderStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            var status = (int)order.Status;

            switch (status)
            {
                case 0:
                    order.Status = (Data.Enums.OrderStatus)1;
                    break;

                case 1:
                    order.Status = (Data.Enums.OrderStatus)2;
                    break;

                case 2:
                    order.Status = (Data.Enums.OrderStatus)3;
                    break;

                case 3:
                    order.Status = (Data.Enums.OrderStatus)4;
                    break;
            }

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }
    }
}