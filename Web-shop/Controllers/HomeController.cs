using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web_shop.Data;
using Web_shop.Models;
using Web_shop.Models.ViewModels;

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
            var vm = new DetailsVM()
            {
                Product = _dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id)
            };

            return View(vm);
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
