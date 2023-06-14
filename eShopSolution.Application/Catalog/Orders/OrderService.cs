using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EShopDbContext _context;

        public OrderService(EShopDbContext context)
        {
            _context = context;
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
                ShipPhoneNumber = request.PhoneNumber
            };

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var orderDt in request.OrderDetails)
            {
                orderDetails.Add(new OrderDetail()
                {
                    OrderId = newOrder.Id,
                    ProductId = orderDt.ProductId,
                    Quantity = orderDt.Quantity,
                    Price = 0
                });
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
                            Address = o.ShipAddress,
                            Email = o.ShipEmail,
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
                                                Quantity = od.Quantity
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
                            Address = o.ShipAddress,
                            Email = o.ShipEmail,
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
                                                Quantity = od.Quantity
                                            }).ToList()
                        };
            return await query.FirstOrDefaultAsync();
        }
    }
}