using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using Braintree;

using Web_shop.Models;
using Web_shop.DataAccess.Repository;
using Web_shop.Utility.BrainTree;
using Web_shop.Models.ViewModels;
using Web_shop.Utility;

namespace Web_shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly IRepository<Product> _prodRepo;
        private readonly IRepository<InquiryHeader> _inqHRepo;
        private readonly IRepository<InquiryDetail> _inqDRepo;
        private readonly IRepository<OrderHeader> _orderHRepo;
        private readonly IRepository<OrderDetail> _orderDRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        
        public CartController(IWebHostEnvironment webHostEnvironment,IEmailSender emailSender,
                              IRepository<ApplicationUser> userRepo, IRepository<Product> prodRepo,
                              IRepository<InquiryHeader> inqHRepo, IRepository<InquiryDetail> inqDRepo,
                              IRepository<OrderHeader> orderHRepo, IRepository<OrderDetail> orderDRepo, 
                              IBrainTreeGate brain)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _brain = brain;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _inqDRepo = inqDRepo;
            _inqHRepo = inqHRepo;
            _orderDRepo = orderDRepo;
            _orderHRepo = orderHRepo;
        }
        
        public IActionResult Index()
        {
            var shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            var prodListTemp = _prodRepo.All.Where(p => prodInCart.Contains(p.Id));
            var prodList = new List<Product>();
            foreach (var cartObj in shoppingCartList)
            {
                var prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempSqFt = cartObj.SqFt;
                prodList.Add(prodTemp);
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            var shoppingCartList = prodList.Select(prod => new ShoppingCart { ProductId = prod.Id, SqFt = prod.TempSqFt });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
          
            return RedirectToAction(nameof(Summary));
        }
                
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if(User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }

                
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);
            }
            
            var gateway = _brain.BraintreeGateway;
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
           
           
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any())
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.All.Where(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                User = applicationUser,
            };

            foreach(var cartObj in shoppingCartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempSqFt = cartObj.SqFt;
                ProductUserVM.Products.Add(prodTemp);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WC.AdminRole))
            {
                var orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = ProductUserVM.Products.Sum(x=>x.TempSqFt*x.Price),
                    City = ProductUserVM.User.City,
                    StreetAddress = ProductUserVM.User.StreetAddress,
                    State = ProductUserVM.User.State,
                    PostalCode = ProductUserVM.User.PostalCode,
                    FullName = ProductUserVM.User.FullName,
                    Email = ProductUserVM.User.Email,
                    PhoneNumber = ProductUserVM.User.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };
                _orderHRepo.Add(orderHeader);
                _orderHRepo.Save();

                var orderDetails = ProductUserVM.Products.Select(prod =>
                new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerSqFt = prod.Price,
                    Sqft = prod.TempSqFt,
                    ProductId = prod.Id
                });
                _orderDRepo.AddRange(orderDetails);
                _orderDRepo.Save();

                var nonceFromTheClient = collection["payment_method_nonce"];
               
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId=orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brain.BraintreeGateway;
                var result = gateway.Transaction.Sale(request);

                if (result.Target?.ProcessorResponseText == WC.StatusApproved)
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }

                _orderHRepo.Save();
                
                return RedirectToAction(nameof(InquiryConfirmation), new { id=orderHeader.Id });
            }
            else
            {
                var pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
               + "templates" + Path.DirectorySeparatorChar.ToString() +
               "Inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                var ProductsSB = new StringBuilder();
                foreach (var prod in ProductUserVM.Products)
                {
                    ProductsSB.Append($" - Name: { prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
                }

                var messageBody = string.Format(HtmlBody,
                                                ProductUserVM.User.FullName,
                                                ProductUserVM.User.Email,
                                                ProductUserVM.User.PhoneNumber,
                                                ProductsSB.ToString());


                await _emailSender.SendEmailAsync(WC.AdminEmail, subject, messageBody);
                var inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.User.FullName,
                    Email = ProductUserVM.User.Email,
                    PhoneNumber = ProductUserVM.User.PhoneNumber,
                    InquiryDate = DateTime.Now

                };

                _inqHRepo.Add(inquiryHeader);
                _inqHRepo.Save();

                var inquiryDetails = ProductUserVM.Products.Select(prod => new InquiryDetail()
                                                                   {
                                                                       InquiryHeaderId = inquiryHeader.Id,
                                                                       ProductId = prod.Id,
                                                                   });
                _inqDRepo.AddRange(inquiryDetails);
                _inqDRepo.Save();
                TempData[WC.Success] = "Inquiry submitted successfully";
            }

            return RedirectToAction(nameof(InquiryConfirmation));
        }
        
        public IActionResult InquiryConfirmation(int id=0)
        {
            var orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id);
            HttpContext.Session.Clear();
            
            return View(orderHeader);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            var shoppingCartList = prodList.Select(prod => new ShoppingCart { ProductId = prod.Id, SqFt = prod.TempSqFt });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index","Home");
        }
    }
}

