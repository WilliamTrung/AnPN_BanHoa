using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task SaveOrderAsync(Order order);

        Task<Order> GetLatestOrderForUserAsync(string userId);

        Task SaveOrderItemAsync(OrderItems orderItem);

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);

        Task<IEnumerable<Order>> GetOrdersAsync();

        Task<IList<OrderItems>> GetOrderItemsByOrderIdAsync(int orderId);

        Task<IEnumerable<OrderItems>> GetOrderItemsAsync();

        Task UpdateOrderItemsAsync(OrderItems orderItems);
    }
}
