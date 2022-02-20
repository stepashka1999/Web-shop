using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_shop.Models.ViewModels
{
   public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderHList { get; set; }
        
        public IEnumerable<SelectListItem> StatusList { get; set; }
        
        public string Status { get; set; }
    }
}
