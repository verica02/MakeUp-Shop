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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace MakeupShop.Controllers
{
    public class MakeupController : Controller
    {
        private readonly MakeupShopContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MakeupController(MakeupShopContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Makeup
        public async Task<IActionResult> Index(string makeupCategory, string searchString)
        {
            IQueryable<Makeup> makeup = _context.Makeup.AsQueryable();
            IQueryable<string> categoryQuery = _context.Makeup.OrderBy(m => m.Category).Select(m => m.Category).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                makeup = makeup.Where(s => s.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(makeupCategory))
            {
                makeup = makeup.Where(x => x.Category == makeupCategory);
            }

            makeup = makeup.Include(m => m.Brand).Include(x => x.Reviews);

            var makeupCategoryVM = new MakeupCategoryViewModel
            {
                Categories = new SelectList(await categoryQuery.ToListAsync()),
                Makeups = await makeup.ToListAsync()
            };

            return View(makeupCategoryVM);
        }

        // GET: Makeup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Makeup == null)
            {
                return NotFound();
            }

            var makeup = await _context.Makeup
                .Include(m => m.Brand).Include(x => x.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (makeup == null)
            {
                return NotFound();
            }

            return View(makeup);
        }

        // GET: Makeup/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Set<Brand>(), "Id", "Name");
            return View();
        }

        // POST: Makeup/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MakeupModel viewmodel)
        {
            string uniqueFileName = UploadedFile(viewmodel);

            await _context.Makeup.AddAsync(new Makeup()
            {
                Name = viewmodel.Makeup.Name,
                Description = viewmodel.Makeup.Description,
                Price = viewmodel.Makeup.Price,
                Image = uniqueFileName,
                Category = viewmodel.Makeup.Category,
                BrandId = viewmodel.Makeup.BrandId
            });
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Makeup/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Makeup == null)
            {
                return NotFound();
            }

            var makeup = await _context.Makeup.FindAsync(id);
            if (makeup == null)
            {
                return NotFound();
            }

            MakeupModel viewmodel = new MakeupModel
            {
                Makeup = makeup
            };

            ViewData["BrandId"] = new SelectList(_context.Set<Brand>(), "Id", "Name", makeup.BrandId);
            return View(viewmodel);
        }

        // POST: Makeup/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MakeupModel viewmodel)
        {
            if (id != viewmodel.Makeup.Id)
            {
                return NotFound();
            }

            string uniqueFileName = UploadedFile(viewmodel);

                try
                {
                    var makeup = _context.Makeup.Where(b => b.Id == id).First();
                    makeup.Image = uniqueFileName;
                    makeup.Name = viewmodel.Makeup.Name;
                    makeup.Description = viewmodel.Makeup.Description;
                    makeup.Price = viewmodel.Makeup.Price;
                    makeup.Category = viewmodel.Makeup.Category;
                    makeup.BrandId = viewmodel.Makeup.BrandId;
                    _context.Update(makeup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MakeupExists(viewmodel.Makeup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

        }

        // GET: Makeup/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Makeup == null)
            {
                return NotFound();
            }

            var makeup = await _context.Makeup
                .Include(m => m.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (makeup == null)
            {
                return NotFound();
            }

            return View(makeup);
        }

        // POST: Makeup/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Makeup == null)
            {
                return Problem("Entity set 'MakeupShopContext.Makeup'  is null.");
            }
            var makeup = await _context.Makeup.FindAsync(id);
            if (makeup != null)
            {
                string photoPath = Path.Combine(webHostEnvironment.WebRootPath, "images/" + makeup.Image);

                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
                _context.Makeup.Remove(makeup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MakeupExists(int id)
        {
          return (_context.Makeup?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string UploadedFile(MakeupModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
