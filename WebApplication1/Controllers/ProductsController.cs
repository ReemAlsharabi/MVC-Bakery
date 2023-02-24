using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Grpc.Core;
using System.IO;
using WebApplication1.Data;
using Microsoft.Extensions.Hosting.Internal;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Product.ToListAsync();
            var product = new List<Product>();

            foreach (var prod in products)
            {
                var categoryName = await _context.Categories
                    .Where(c => c.Id == prod.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();

                var productViewModel = new Product
                {
                    Id = prod.Id,
                    Name = prod.Name,
                    CategoryName = categoryName,
                    Description = prod.Description,
                    Image = prod.Image,
                    ImageURL = prod.ImageURL,
                    Available = prod.Available,
                    CategoryId = prod.CategoryId,
                    Price = prod.Price
                };

                product.Add(productViewModel);
            }

            return View(product);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            // Fetch the category name based on the product's category ID
            var categoryName = await _context.Categories
                .Where(c => c.Id == product.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
            product.CategoryName = categoryName;
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            GetCategoryName();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Image,Available,CategoryId")] Product product)
        {
            GetCategoryName();

            if (ModelState.IsValid)
            {
                if (product.Image != null)
                {
                    var uniqueFileName = GetUniqueFileName(product.Image.FileName);
                    var uploads = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    product.ImageURL = "/images/" + uniqueFileName;
                    product.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            GetCategoryName();
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Image,Available,CategoryId")] Product product)
        {
            GetCategoryName();
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.Image != null)
                    {
                        var uniqueFileName = GetUniqueFileName(product.Image.FileName);
                        var uploads = Path.Combine(_webHostEnviroment.WebRootPath, "images");
                        var oldFilePath = Path.Combine(_webHostEnviroment.WebRootPath, product.ImageURL.TrimStart('/'));
                        var filePath = Path.Combine(uploads, uniqueFileName);
                        product.ImageURL = "/images/" + uniqueFileName;
                        product.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product != null)
            {
                var filePath = Path.Combine(_webHostEnviroment.WebRootPath, product.ImageURL.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
        public void GetCategoryName()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + System.Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }
}