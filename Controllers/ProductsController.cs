using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecondHandPlatform.Data;
using SecondHandPlatform.Models;
using System.Diagnostics;

namespace SecondHandPlatform.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => 
                    p.Title.Contains(search) || 
                    p.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SearchTerm = search;
            ViewBag.CategoryId = categoryId;

            return View(await products.OrderByDescending(p => p.UploadDate).ToListAsync());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/MyProducts
        [Authorize]
        public async Task<IActionResult> MyProducts()
        {
            var userId = _userManager.GetUserId(User);
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            var userId = _userManager.GetUserId(User);
            product.UserId = userId;
            product.UploadDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate file size (5MB max)
                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageFile", "File size must be less than 5MB");
                        ViewData["Categories"] = _context.Categories.ToList();
                        return View(product);
                    }

                    // Validate file extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Only image files are allowed (jpg, jpeg, png, gif)");
                        ViewData["Categories"] = _context.Categories.ToList();
                        return View(product);
                    }

                    // Create uploads directory if it doesn't exist
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = $"/uploads/{uniqueFileName}";
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyProducts));
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.UserId != userId)
            {
                return Forbid();
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null || existingProduct.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload if new image provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Validate file
                        if (imageFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageFile", "File size must be less than 5MB");
                            ViewData["Categories"] = _context.Categories.ToList();
                            return View(product);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ImageFile", "Only image files are allowed");
                            ViewData["Categories"] = _context.Categories.ToList();
                            return View(product);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                        {
                            var oldFilePath = Path.Combine(_environment.WebRootPath, existingProduct.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save new image
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingProduct.ImagePath = $"/uploads/{uniqueFileName}";
                    }

                    // Update other properties
                    existingProduct.Title = product.Title;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;

                    _context.Update(existingProduct);
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
                return RedirectToAction(nameof(MyProducts));
            }

            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.UserId != userId)
            {
                return Forbid();
            }

            // Delete image file if exists
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var filePath = Path.Combine(_environment.WebRootPath, product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyProducts));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}