using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _catRepo;

        public CategoryController(IRepository<Category> catRepo)
        {
            _catRepo = catRepo;
        }

        public IActionResult Index() => View(_catRepo.All);

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(obj);
                _catRepo.Save();
                TempData[WC.Success] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData[WC.Error] = "Error while creating category";

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if(!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());

            return obj is null ? NotFound() : View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(obj);
                _catRepo.Save();
                TempData[WC.Success] = "Action completed successfully";

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());
            
            return obj is null ? NotFound() : View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            TempData[WC.Success] = "Action completed successfully";
            _catRepo.Remove(obj);
            _catRepo.Save();
        
            return RedirectToAction("Index");
        }
    }
}
