using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerce.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _order;

        /// <summary>
        /// The constructor to bring in IOrder interface that enables getting information from the order data table
        /// </summary>
        /// <param name="order"></param>
        public IndexModel(IOrderService order)
        {
            _order = order;
        }

        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderItems> OrderItems { get; set; }

        /// <summary>
        /// Get all the orders from the order table
        /// Get all the order items from the each order
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            Orders = await _order.GetOrdersAsync();
            OrderItems = await _order.GetOrderItemsAsync();
        }
    }
}
