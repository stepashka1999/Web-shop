using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IRepository<InquiryHeader> _inqHRepo;
        private readonly IRepository<InquiryDetail> _inqDRepo;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IRepository<InquiryDetail> inqDRepo, IRepository<InquiryHeader> inqHRepo)
        {
            _inqDRepo = inqDRepo;
            _inqHRepo = inqHRepo;
        }

        public IActionResult Index() => View();
        
        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == id),
                InquiryDetails = _inqDRepo.All.Where(u => u.InquiryHeaderId == id).Include(u => u.Product)
            };

            return View(InquiryVM);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            InquiryVM.InquiryDetails = _inqDRepo.All.Where(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
            var shoppingCartList = InquiryVM.InquiryDetails.Select(detail => new ShoppingCart()
                                                                  {
                                                                      ProductId = detail.ProductId,
                                                                      SqFt = 1
                                                                  });
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDRepo.All.Where(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inqDRepo.RemoveRange(inquiryDetails);
            _inqHRepo.Remove(inquiryHeader);
            _inqHRepo.Save();
            TempData[WC.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList() => Json(new { data = _inqHRepo.All });
        
        #endregion
    }
}
