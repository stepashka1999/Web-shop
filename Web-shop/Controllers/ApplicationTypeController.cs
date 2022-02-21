using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IRepository<ApplicationType> _appTypeRepo;

        public ApplicationTypeController(IRepository<ApplicationType> appTypeRepo)
        {
            _appTypeRepo = appTypeRepo;
        }

        public IActionResult Index() => View(_appTypeRepo.All);

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Add(obj);
                _appTypeRepo.Save();
                TempData[WC.Success] = "Action completed successfully";
                
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var obj = _appTypeRepo.Find(id.GetValueOrDefault());

            return obj is null ? NotFound() : View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Update(obj);
                _appTypeRepo.Save();
                TempData[WC.Success] = "Action completed successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            
            return obj is null ? NotFound() : View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
        
            _appTypeRepo.Remove(obj);
            _appTypeRepo.Save();
            TempData[WC.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
