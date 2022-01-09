using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_shop.Models.ViewModels
{
    public class InquieryVM
    {
        public InquieryHeader InquieryHeader { get; set; }

        public IEnumerable<InquieryDetail> InquieryDetails { get; set; }
    }
}
