using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecondHandPlatform.Data;
using SecondHandPlatform.Models;
using System.Diagnostics;

namespace SecondHandPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .OrderByDescending(p => p.UploadDate)
                .Take(12)
                .ToListAsync();
            
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult DebugForm()
{
    return View();
}

[HttpPost]
public IActionResult DebugForm(string name, string description)
{
    ViewBag.Message = $"Received - Name: {name}, Description: {description}";
    return View();
}



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}