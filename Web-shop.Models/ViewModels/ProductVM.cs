using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web_shop.Models.ViewModels
{
    public class ProductVM
    {
        public IEnumerable<SelectListItem> Categories { get; set; }

        public Product Product { get; set; }
    }
}
