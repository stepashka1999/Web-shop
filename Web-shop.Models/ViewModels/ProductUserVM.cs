using System.Collections.Generic;

namespace Web_shop.Models.ViewModels
{
    public class ProductUserVM
    {
        public ApplicationUser User { get; set; }

        public IList<Product> Products { get; set; } = new List<Product>();
    }
}
