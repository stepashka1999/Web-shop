using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_shop.Models
{
    public class InquieryDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InquieryHeaderId { get; set; }

        [ForeignKey(nameof(InquieryHeaderId))]
        public InquieryHeader InquieryHeader { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
