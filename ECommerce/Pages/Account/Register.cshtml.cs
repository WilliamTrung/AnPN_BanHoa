using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Core.Constants;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace ECommerce.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IShopService _shop;
        private IEmailService _emailService;

        public IConfiguration Configuration { get; }

        /// <summary>
        /// A property to be available on the Model property in the Razor Page
        /// It uses BindProperty attribute to access the values outside of the handler method
        /// </summary>
        [BindProperty]
        public RegisterInput Input { get; set; }

        /// <summary>
        /// A property that brings in SignInManager depdency to be used in the class
        /// </summary>
        /// <param name="userManager">UserManager context</param>
        /// <param name="signInManager">SignInManager context</param>
        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IShopService shop, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            Configuration = configuration;
            _shop = shop;
            _emailService = emailService;
        }

        /// <summary>
        /// A handler method to process the default GET request
        /// </summary>
        public void OnGet()
        {

        }

        /// <summary>
        /// A handler method to process a POST request after a user's registration information has been entered
        /// </summary>
        /// <returns>Home page upon successful registration</returns>
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Address = Input.Address,
                    Address2 = Input.Address2,
                    City = Input.City,
                    State = Input.State,
                    Zip = Input.Zip
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    Claim name = new Claim("FullName", $"{Input.FirstName} {Input.LastName}");
                    Claim email = new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email);

                    List<Claim> claims = new List<Claim> { name, email };
                    await _userManager.AddClaimsAsync(user, claims);

                    string adminEmail = Configuration["ADMIN_EMAIL"];

                    if (Input.Email == adminEmail)
                    {
                        await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
                    }

                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Member);

                    var cart = new Cart
                    {
                        UserID = user.Id
                    };

                    await _shop.CreateCartAsync(cart);

                    string subject = Subject.WelcomeMessage;
                    string message =
                        $"<p>Hello {user.FirstName} {user.LastName},</p>" +
                        $"<p>&nbsp;</p>" +
                        $"<p>You have successfully created a new account.</p>";

                    await _emailService.SendEmailAsync(user.Email, subject, message);

                    await _signInManager.SignInAsync(user, false);

                    Response.Redirect("/");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        /// <summary>
        /// A class to define the RegisterInput
        /// </summary>
        public class RegisterInput
        {
            [Required]
            [Display(Name = "First Name:")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name:")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address:")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
            [Display(Name = "Password:")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            [Display(Name = "Confirm Password:")]
            public string ConfirmPassword { get; set; }

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
        }
    }
}
