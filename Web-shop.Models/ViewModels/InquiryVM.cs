using System.Collections.Generic;

namespace Web_shop.Models.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }

        public IEnumerable<InquiryDetail> InquiryDetails { get; set; }
    }
}
