using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MakeupShop.Data;
using MakeupShop.Models;
using MakeupShop.ViewModels;

namespace MakeupShop.Controllers
{
    public class UserMakeupController : Controller
    {
        private readonly MakeupShopContext _context;

        public UserMakeupController(MakeupShopContext context)
        {
            _context = context;
        }

        // GET: UserMakeup
        public async Task<IActionResult> Index()
        {
            var makeupShopContext = _context.UserMakeup.Where(u => u.AppUser == User.Identity.Name).Include(u => u.Makeup);
            return View(await makeupShopContext.ToListAsync());
        }

        public async Task<IActionResult> Buy(int? id, UserMakeupVM viewmodel)
        {
            var makeup = _context.Makeup.Where(m => m.Id == id).FirstOrDefault();
            ViewBag.MakeupName = makeup.Name;
            ViewBag.MakeupId = id;
            ViewBag.Price = makeup.Price;
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(UserMakeupVM viewmodel)
        {
            await _context.UserMakeup.AddAsync(new UserMakeup()
            {
                AppUser = viewmodel.UserMakeup.AppUser,
                MakeupId = viewmodel.MakeupId
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Makeup");
        }

        private bool UserMakeupExists(int id)
        {
          return (_context.UserMakeup?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
