using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web_shop.Data;
using Web_shop.Models;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var shoppingCart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            shoppingCart ??= Enumerable.Empty<ShoppingCart>();
            var products = _dbContext.Products.AsEnumerable().Join(shoppingCart,
                                                    product => product.Id,
                                                    cart => cart.ProductId,
                                                    (product, cart) => product);

            return View(products);
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

            return RedirectToAction(nameof(Index));
        }
    }
}
