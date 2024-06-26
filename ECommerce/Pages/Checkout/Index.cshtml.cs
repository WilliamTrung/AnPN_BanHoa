using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Constants;

namespace ECommerce.Pages.Checkout
{
    /// <summary>
    /// Inherits from PageModel class and brings in dependencies including UserManager, IEmailSender interface, and IShop interface
    /// Create a CheckoutInput class and set getter and setter
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IShopService _shop;
        private readonly IOrderService _order;

        public IndexModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IShopService shop,
            IOrderService order)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _shop = shop;
            _order = order;
        }

        [BindProperty]
        public CheckoutInput Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);

                Order order = new Order
                {
                    UserID = user.Id,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Address = Input.Address,
                    Address2 = Input.Address2,
                    City = Input.City,
                    State = Input.State,
                    Zip = Input.Zip,
                    CreditCard = Input.CreditCard.ToString(),
                    Timestamp = DateTime.Now.ToString()
                };

                await _order.SaveOrderAsync(order);

                order = await _order.GetLatestOrderForUserAsync(user.Id);

                IEnumerable<CartItems> cartItems = await _shop.GetCartItemsByUserIdAsync(user.Id);
                IList<OrderItems> orderItems = new List<OrderItems>();
                decimal total = 0;

                foreach (var cartItem in cartItems)
                {
                    OrderItems orderItem = new OrderItems
                    {
                        OrderID = order.ID,
                        ProductID = cartItem.ProductID,
                        Quantity = cartItem.Quantity
                    };
                    orderItems.Add(orderItem);
                    total += cartItem.Product.Price * cartItem.Quantity;
                }

                double finalCost = Decimal.ToDouble(total) * 1.1;
                foreach (var item in orderItems)
                {
                    await _order.SaveOrderItemAsync(item);
                }

                string subject = Subject.NotifyPurchasementMessage;
                string message =
                    $"<p>Hello {user.FirstName} {user.LastName},</p>" +
                    $"<p>&nbsp;</p>" +
                    $"<p>Below is your recent purchase summary</p>" +
                    $"<p>Total: ${finalCost.ToString("F")}\n</p>";

                await _emailSender.SendEmailAsync(user.Email, subject, message);
                await _shop.RemoveCartItemsAsync(cartItems);

                return Redirect("/Checkout/Receipt");
            }
            return Page();
        }

        public class CheckoutInput
        {
            [Display(Name = "Purchased Date:")]
            public DateTime Date { get; set; }

            [Required]
            [Display(Name = "First Name:")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name:")]
            public string LastName { get; set; }

            [Required]
            public string Address { get; set; }

            [Display(Name = "Address 2:")]
            public string Address2 { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string State { get; set; }

            [Required]
            [DataType(DataType.PostalCode)]
            [Compare("Zip", ErrorMessage = "The is an invalid zip code")]
            public string Zip { get; set; }

            [Required]
            public CreditCard CreditCard { get; set; }
        }

        public enum CreditCard
        {
            Visa = 0,
            Mastercard,
            [Display(Name = "Amenrican Express")]
            AmericanExpress
        }
    }
}
