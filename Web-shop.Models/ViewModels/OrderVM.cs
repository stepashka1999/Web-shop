using System.Collections.Generic;

namespace Web_shop.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
