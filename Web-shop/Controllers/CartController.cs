using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web_shop.DataAccess.Data;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ApplicationUser> _applicationUserRepository;
        private readonly IRepository<InquieryHeader> _inqueryHeaderRepository;
        private readonly IRepository<InquieryDetail> _inqueryDetailRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IRepository<Product> productRepository, 
                              IRepository<ApplicationUser> userRepository, 
                              IRepository<InquieryHeader> inqueryHeaderRepository,
                              IRepository<InquieryDetail> inqueryDetailRepository,
                              IWebHostEnvironment env,
                              IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _applicationUserRepository = userRepository;
            _inqueryHeaderRepository = inqueryHeaderRepository;
            _inqueryDetailRepository = inqueryDetailRepository;
            _webHostEnvironment = env;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var shoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            shoppingCart ??= Enumerable.Empty<ShoppingCart>();
            var products = _productRepository.All.AsEnumerable().Join(shoppingCart,
                                                    product => product.Id,
                                                    cart => cart.ProductId,
                                                    (product, cart) => product);

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            shoppingCart ??= Enumerable.Empty<ShoppingCart>();

            var products = _productRepository.All
                                             .AsEnumerable()
                                             .Join(shoppingCart,
                                                    product => product.Id,
                                                    cart => cart.ProductId,
                                                    (product, cart) => product);

            ProductUserVM = new ProductUserVM()
            {
                Products = products.ToList(),
                User = _applicationUserRepository.FirstOrDefault(u => u.Id == userId)
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var pathToTemplate = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "Inquiry.html");
            var subject = "New Iquiry";
            var templateText = System.IO.File.ReadAllText(pathToTemplate);

            var sb = new StringBuilder();
            foreach(var product in ProductUserVM.Products)
            {
                sb.Append($"- Name: {product.Name} <span style=\"font-size:14px;\">(ID: {product.Id})<span/><br/>");
            }

            var productsString = sb.ToString();
            var htmlBody = string.Format(templateText, productUserVM.User.FullName, productUserVM.User.Email, productUserVM.User.PhoneNumber, productsString);

            await _emailSender.SendEmailAsync(WC.AdminEmail, subject, htmlBody);

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var inqueryHeader = new InquieryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = productUserVM.User.FullName,
                Email = productUserVM.User.Email,
                PhoneNumber = productUserVM.User.PhoneNumber,
                InquireDate = System.DateTime.Now
            };
            _inqueryHeaderRepository.Add(inqueryHeader);
            _inqueryHeaderRepository.Save();

            foreach(var prod in ProductUserVM.Products) 
            {
                var inquieryDetail = new InquieryDetail()
                {
                    InquieryHeaderId = inqueryHeader.Id,
                    ProductId = prod.Id
                };
                _inqueryDetailRepository.Add(inquieryDetail);
            }

            _inqueryDetailRepository.Save();

            return RedirectToAction(nameof(InqueryConfirmation));
        }

        public IActionResult InqueryConfirmation()
        {
            HttpContext.Session.Clear();

            return View();
        }

        public IActionResult Remove(int id)
        {
            var sessionCart = HttpContext.Session.Get<IList<ShoppingCart>>(WC.SessionCart);
            if (sessionCart is null || !sessionCart.Any(c => c.ProductId == id))
            {
                return NotFound();
            }

            var cart = sessionCart.FirstOrDefault(c => c.ProductId == id);
            sessionCart.Remove(cart);
            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WC.SessionCart, sessionCart);
            TempData[WC.SuccessNotification] = $"Product with id {cart.ProductId} removed from cart";

            return RedirectToAction(nameof(Index));
        }
    }
}
