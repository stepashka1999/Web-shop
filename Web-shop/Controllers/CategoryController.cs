using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_shop.DataAccess.Data;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> repository)
        {
            _categoryRepository = repository;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.All;

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _categoryRepository.Add(category);
            _categoryRepository.Save();
            TempData[WC.SuccessNotification] = "Category added successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

            var category = _categoryRepository.Find(id);
            
            return category is null ? NotFound() : View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _categoryRepository.Update(category);
            _categoryRepository.Save();
            TempData[WC.SuccessNotification] = $"Category with id equals {category.Id} updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }

            var category = _categoryRepository.Find(id);
            if(category is null)
            {
                TempData[WC.ErrorNotification] = $"Category with id equals {id} not found";

                return NotFound();
            }

            _categoryRepository.Remove(category);
            _categoryRepository.Save();
            TempData[WC.SuccessNotification] = $"Category with id equals {id} deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
