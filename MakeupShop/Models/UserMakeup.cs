using System.ComponentModel.DataAnnotations;

namespace MakeupShop.Models
{
    public class UserMakeup
    {
        public int Id { get; set; }

        [StringLength(450)]
        public string AppUser { get; set; }

        public int MakeupId { get; set; }
        public Makeup? Makeup { get; set; }
    }
}
