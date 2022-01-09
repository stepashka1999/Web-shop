using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Web_shop.DataAccess.Data;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IWebHostEnvironment eviroment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = eviroment;
        }

        public IActionResult Index()
        {
            var productst = _productRepository.All.Include(x => x.Category);
            return View(productst);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                Categories = _categoryRepository.All.AsEnumerable().Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() })
            };

            if(id is null)
            {
                TempData[WC.ErrorNotification] = $"Product id was null";

                return View(productVM);
            }

            var product = _productRepository.Find(id.Value);
            if(product is null)
            {
                TempData[WC.ErrorNotification] = $"Product with id equals {id.Value} not found";

                return NotFound();
            }

            productVM.Product = product;

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(!ModelState.IsValid)
            {
                return View(productVM);
            }
            
            //Update
            if (productVM.Product.Id != 0)
            {
                var product = productVM.Product;
                //File logic
                var oldProduct = _productRepository.All.AsNoTracking().FirstOrDefault(p => p.Id == product.Id);
                product.ImageLink = oldProduct.ImageLink;
                if (HttpContext.Request.Form.Files.Any())
                {
                    var file = HttpContext.Request.Form.Files[0];
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var fileStream = System.IO.File.Create(Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, newFileName));
                    file.CopyTo(fileStream);
                    product.ImageLink = newFileName;

                    if (oldProduct.ImageLink is not null)
                    {
                        var oldFileFullPath = Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, oldProduct.ImageLink);
                        if (System.IO.File.Exists(oldFileFullPath))
                        {
                            System.IO.File.Delete(oldFileFullPath);
                        }
                    }
                }

                _productRepository.Update(product);
                _productRepository.Save();
                TempData[WC.SuccessNotification] = $"Product was successfully updated";

                return RedirectToAction(nameof(Index));
            }
                        
            _productRepository.Add(productVM.Product);
            _productRepository.Save();
            TempData[WC.SuccessNotification] = $"Product was successfully created";


            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var product = _productRepository.All.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                TempData[WC.ErrorNotification] = $"Product with id equals {id} not found";

                return NotFound();
            }

            if (product.ImageLink is not null)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, product.ImageLink);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _productRepository.Remove(product);
            _productRepository.Save();
            TempData[WC.ErrorNotification] = $"Product with id equals {id} was successfully deleted";


            return RedirectToAction(nameof(Index));
        }
    }
}
