using Bulky.BAL.Models;
using Bulky.BAL.Models.ViewModels;
using Bulky.DAL.Repository;
using Bulky.DAL.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll().ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> catergoyList = _unitOfWork.CategoryRepository
            .GetAll()
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            });

            var pv = new ProductViewModel() { Product = new Product(), CategoryList = catergoyList };


            return View(pv);
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(pvm.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                pvm.CategoryList = _unitOfWork.CategoryRepository
                .GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });
            }
            return View(pvm);
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if (product is null)
            {
                return NotFound();
            }


            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var product = _unitOfWork.ProductRepository.Get(u => u.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var product = _unitOfWork.ProductRepository.Get(u => u.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }


    }
}
