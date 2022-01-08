using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Web_shop.Data;
using Web_shop.Models;
using Web_shop.Models.ViewModels;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment eviroment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = eviroment;
        }

        public IActionResult Index()
        {
            var productst = _dbContext.Products.Include(x => x.Category);
            return View(productst);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                Categories = _dbContext.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() })
            };

            if(id is null)
            {
                return View(productVM);
            }

            var product = _dbContext.Products.Find(id);
            if(product is null)
            {
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
                var oldProduct = _dbContext.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id);
                if (HttpContext.Request.Form.Files.Any())
                {
                    var file = HttpContext.Request.Form.Files[0];
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var fileStream = System.IO.File.Create(Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, newFileName));
                    file.CopyTo(fileStream);

                    var oldFileFullPath = Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, oldProduct.ImageLink);
                    if (System.IO.File.Exists(oldFileFullPath))
                    {
                        System.IO.File.Delete(oldFileFullPath);
                    }
                                            
                    product.ImageLink = newFileName;
                }

                product.ImageLink = oldProduct.ImageLink;
                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
                        
            _dbContext.Products.Add(productVM.Product);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var product = _dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }

            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, WC.ImagePath, product.ImageLink);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
