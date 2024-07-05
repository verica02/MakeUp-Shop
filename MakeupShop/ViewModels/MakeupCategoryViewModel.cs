using MakeupShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MakeupShop.ViewModels
{
    public class MakeupCategoryViewModel
    {
        public IList<Makeup> Makeups { get; set; }
        public SelectList Categories { get; set; }
        public string MakeupCategory { get; set; }
        public string SearchString { get; set; }

    }
}
