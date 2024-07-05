using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MakeupShop.Models;

namespace MakeupShop.Views.UserMakeup
{

        public class DeleteModel : PageModel
        {
            private readonly MakeupShop.Data.MakeupShopContext _context;

            public DeleteModel(MakeupShop.Data.MakeupShopContext context)
            {
                _context = context;
            }

            [BindProperty]
            public Makeup Makeup{ get; set; } = default!;

            public async Task<IActionResult> OnGetAsync(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var kniga = await _context.Makeup.FirstOrDefaultAsync(m => m.Id == id);

                if (kniga == null)
                {
                    return NotFound();
                }
                else
                {
                    Makeup = kniga;
                }
                return Page();
            }

            public async Task<IActionResult> OnPostAsync(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var kniga = await _context.Makeup.FindAsync(id);
                if (kniga != null)
                {
                    Makeup = kniga;
                    _context.Makeup.Remove(Makeup);
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("./Index");
            }
       }
    
}
