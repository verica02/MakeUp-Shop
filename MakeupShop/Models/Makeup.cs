using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MakeupShop.Models
{
    public class Makeup
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        public string? Image { get; set; }

        [StringLength(30)]
        public string Category { get; set; }

        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<UserMakeup> UserMakeup { get; set; }

    }
}
