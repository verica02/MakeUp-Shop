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
using System.Data;

namespace MakeupShop.Controllers
{
    public class BrandsController : Controller
    {
        private readonly MakeupShopContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BrandsController(MakeupShopContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Brands
        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<Brand> brands = _context.Brand.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                brands = brands.Where(s => s.Name.Contains(searchString));
            }

            var brandTitleVM = new BrandTitleViewModel
            {
                Brands = await brands.ToListAsync()
            };

            return View(brandTitleVM);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel viewmodel)
        {
            string uniqueFileName = UploadedFile(viewmodel);

            await _context.Brand.AddAsync(new Brand()
            {
                Name = viewmodel.Brand.Name,
                Description = viewmodel.Brand.Description,
                ReleaseDate = viewmodel.Brand.ReleaseDate,
                Image = uniqueFileName
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            BrandModel viewmodel = new BrandModel
            {
                Brand = brand
            };
            return View(viewmodel);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandModel viewmodel)
        {
            if (id != viewmodel.Brand.Id)
            {
                return NotFound();
            }

            string uniqueFileName = UploadedFile(viewmodel);

            try
            {
                var brand = _context.Brand.Where(b => b.Id == id).First();
                brand.Image = uniqueFileName;
                brand.Name = viewmodel.Brand.Name;
                brand.Description = viewmodel.Brand.Description;
                brand.ReleaseDate = viewmodel.Brand.ReleaseDate;
                
                _context.Update(brand);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(viewmodel.Brand.Id))
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

        // GET: Brands/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Brand == null)
            {
                return Problem("Entity set 'MakeupShopContext.Brand'  is null.");
            }
            var brand = await _context.Brand.FindAsync(id);
            if (brand != null)
            {
                string photoPath = Path.Combine(webHostEnvironment.WebRootPath, "images/" + brand.Image);

                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
                _context.Brand.Remove(brand);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
          return (_context.Brand?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string UploadedFile(BrandModel model)
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

        public int? myID { get; set; }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload(int? id, UploadViewModel viewmodel)
        {
            myID = id;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel viewmodel)
        {
            if(viewmodel.file != null)
            {
                if(viewmodel.file.Length > 0 && viewmodel.file.Length < 10000000)
                {
                    var brand = _context.Brand.FirstOrDefault(x=>x.Id == viewmodel.ID);

                    using (var target = new MemoryStream())
                    {
                        viewmodel.file.CopyTo(target);
                        brand.CatalogDownloadUrl = target.ToArray();
                    }

                    _context.Brand.Update(brand);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            var brand = await _context.Brand.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            if (brand.CatalogDownloadUrl == null)
            {
                return NotFound();
            }
            else
            {
                string mimeType = "application/pdf";
                byte[] byteArr = brand.CatalogDownloadUrl;
                return new FileContentResult(byteArr, mimeType)
                {
                    FileDownloadName = $"{brand.Name} Catalog.pdf"
                };
            }
        }

        public async Task<IActionResult> ShowMakeup(int? id)
        {
            var makeupContext = _context.Makeup.Where(m => m.BrandId == id).Include(u => u.Brand);
            var brand = _context.Brand.Where(b => b.Id == id).FirstOrDefault();
            
            if (brand != null)
            {
                ViewBag.Message = brand.Name;
            }

            return View(await makeupContext.ToListAsync());
        }

    }
}
