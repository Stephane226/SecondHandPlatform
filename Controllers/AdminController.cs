using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecondHandPlatform.Data;
using SecondHandPlatform.Models;
using SecondHandPlatform.ViewModels;

namespace SecondHandPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                RecentProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .OrderByDescending(p => p.UploadDate)
                    .Take(10)
                    .ToListAsync()
            };
            
            return View(stats);
        }

        // Categories Management
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

     
     [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateCategory(Category category)
{
    Console.WriteLine($"CreateCategory POST called. Name: {category?.Name}, Description: {category?.Description}");
    
    if (ModelState.IsValid)
    {
        Console.WriteLine("Model is valid. Saving to database...");
        
        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Category saved with ID: {category.Id}");
            TempData["SuccessMessage"] = $"Category '{category.Name}' created successfully!";
            
            return RedirectToAction(nameof(Categories));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving category: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while saving. Please try again.");
        }
    }
    else
    {
        Console.WriteLine("Model is invalid. Errors:");
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($"- {error.ErrorMessage}");
        }
    }
    
    // If we got here, something failed
    return View(category);
}


        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Check if category is being used by any products
                var productsWithCategory = await _context.Products
                    .AnyAsync(p => p.CategoryId == id);
                
                if (productsWithCategory)
                {
                    TempData["ErrorMessage"] = "Cannot delete category. There are products associated with this category.";
                    return RedirectToAction(nameof(Categories));
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Categories));
        }

        // Products Management
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
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
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            return RedirectToAction(nameof(Products));
        }

        // Users Management
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRolesViewModel>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                     IsLockedOut = user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now 

                });
            }
            
            return View(userRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnabled = !user.LockoutEnabled;
                user.LockoutEnd = user.LockoutEnabled ? 
                    DateTimeOffset.MaxValue : 
                    null;
                
                await _userManager.UpdateAsync(user);
                
                TempData["SuccessMessage"] = user.LockoutEnabled ? 
                    $"User {user.Email} has been locked out." : 
                    $"User {user.Email} has been unlocked.";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdminRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                
                if (isAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                    TempData["SuccessMessage"] = $"Admin privileges removed from {user.Email}.";
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    TempData["SuccessMessage"] = $"{user.Email} is now an Admin.";
                }
            }
            return RedirectToAction(nameof(Users));
        }

        // User Details view
        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userProducts = await _context.Products
                .Where(p => p.UserId == id)
                .Include(p => p.Category)
                .ToListAsync();

            var model = new UserDetailsViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Roles = roles.ToList(),
                Products = userProducts,
                IsLockedOut = user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now
            };

            return View(model);
        }
    }
}