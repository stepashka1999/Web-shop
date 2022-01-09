using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquieryController : Controller
    {
        private readonly IRepository<InquieryHeader> _inqueryHeaderRepository;
        private readonly IRepository<InquieryDetail> _inqueryDetailRepository;

        [BindProperty]
        public InquieryVM InquieryVM { get; set; }

        public InquieryController(IRepository<InquieryHeader> inqueryHeaderRepository, IRepository<InquieryDetail> inqueryDetailRepository)
        {
            _inqueryHeaderRepository = inqueryHeaderRepository;
            _inqueryDetailRepository = inqueryDetailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquieryVM = new InquieryVM()
            {
                InquieryHeader = _inqueryHeaderRepository.Find(id),
                InquieryDetails = _inqueryDetailRepository.All.AsNoTracking().Where(d => d.ProductId == id).Include(d => d.Product)
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            InquieryVM.InquieryDetails = _inqueryDetailRepository.All.AsNoTracking().Where(d => d.ProductId == InquieryVM.InquieryHeader.Id);
            var shoppingCart = InquieryVM.InquieryDetails.Select(d => new ShoppingCart() { ProductId = d.ProductId });
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCart);
            HttpContext.Session.Set(WC.SessionInquieryId, InquieryVM.InquieryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            var inquiertHeader = _inqueryHeaderRepository.Find(InquieryVM.InquieryHeader.Id);
            var inquieryDetails = _inqueryDetailRepository.FindAll(d => d.InquieryHeaderId == inquiertHeader.Id);

            _inqueryHeaderRepository.Remove(inquiertHeader);
            _inqueryDetailRepository.Remove(inquieryDetails);
            _inqueryDetailRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API

        [HttpGet]
        public IActionResult GetInquieryList()
        {
            return Json(new { data = _inqueryHeaderRepository.All.AsEnumerable() });
        }

        #endregion
    }
}
