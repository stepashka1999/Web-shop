using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_shop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order fro category must be greater than 0")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
