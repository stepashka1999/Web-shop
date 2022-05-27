using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public ProductController(IRepository<Product> prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var objList = _prodRepo.All.Include(p => p.Category)
                                       .Include(p => p.ApplicationType);

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            var productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetDropdownList(WC.CategoryName),
                ApplicationTypeSelectList = _prodRepo.GetDropdownList(WC.ApplicationTypeName),
            };

            if (id is null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _prodRepo.Find(id.GetValueOrDefault());

                return productVM.Product is null ? NotFound() : View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                var webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    var upload = Path.Combine(webRootPath, WC.ImagePath);
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _prodRepo.Add(productVM.Product);
                }
                else
                {
                    var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == productVM.Product.Id,isTracking:false);
                    if (files.Any())
                    {
                        var upload = Path.Combine(webRootPath,WC.ImagePath);
                        var fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                                                
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }

                    _prodRepo.Update(productVM.Product);
                }

                TempData[WC.Success] = "Action completed successfully";
                _prodRepo.Save();
                
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _prodRepo.GetDropdownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList = _prodRepo.GetDropdownList(WC.ApplicationTypeName);
         
            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var product = _prodRepo.All.Include(p => p.Category)
                                       .Include(p => p.ApplicationType)
                                       .FirstOrDefault(u => u.Id == id);

            return product is null ? NotFound() : View(product);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            var upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _prodRepo.Remove(obj);
            _prodRepo.Save();
            TempData[WC.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
