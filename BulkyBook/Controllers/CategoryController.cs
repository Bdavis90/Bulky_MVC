using BulkyBook.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Models.Category> categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot match the Name");
            }

            if (ModelState.IsValid)
            {
                _context.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }


    }
}
