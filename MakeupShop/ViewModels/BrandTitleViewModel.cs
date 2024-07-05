using MakeupShop.Models;

namespace MakeupShop.ViewModels
{
    public class BrandTitleViewModel
    {
        public IList<Brand> Brands { get; set; }
        public string SearchString { get; set; }
    }
}
