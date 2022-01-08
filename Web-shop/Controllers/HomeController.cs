using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Web_shop.Data;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM()
            {
                Products = _dbContext.Products.Include(p => p.Category),
                Categories = _dbContext.Categories
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            var sesionCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            var vm = new DetailsVM()
            {
                Product = _dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id),
                ExistInCart = sesionCart is not null && sesionCart.Any(c => c.ProductId == id)
            };

            return View(vm);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            var shoppingCartList = HttpContext.Session.Get<IList<ShoppingCart>>(WC.SessionCart);
            if (shoppingCartList is null)
            {
                shoppingCartList = new List<ShoppingCart>();
            }

            shoppingCartList.Add(new ShoppingCart() { ProductId = id });

            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            var sessionCart = HttpContext.Session.Get<IList<ShoppingCart>>(WC.SessionCart);
            if(sessionCart is null || !sessionCart.Any(c => c.ProductId == id))
            {
                return NotFound();
            }

            var cart = sessionCart.FirstOrDefault(c => c.ProductId == id);
            sessionCart.Remove(cart);
            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WC.SessionCart, sessionCart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
