using Bulky.BAL.Models;
using Bulky.BAL.Models.ViewModels;
using Bulky.DAL.Repository.Interface;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> catergoyList = _unitOfWork.CategoryRepository
            .GetAll()
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            });

            var pv = new ProductViewModel() { Product = new Product(), CategoryList = catergoyList };

            if (id is null || id == 0)
            {
                return View(pv);

            }
            else
            {
                pv.Product = _unitOfWork.ProductRepository.Get(x => x.Id == id);
                return View(pv);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel pvm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(pvm.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, pvm.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    pvm.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (pvm.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(pvm.Product);
                    TempData["success"] = "Product created successfully";

                }
                else
                {
                    _unitOfWork.ProductRepository.Update(pvm.Product);
                    TempData["success"] = "Product updated successfully";

                }
                _unitOfWork.Save();

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

        #region API CALLS
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
        //    return Json(new { data = products });
        //}

        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    var product = _unitOfWork.ProductRepository.Get(x => x.Id == id);
        //    if (product is null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });

        //    }

        //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    _unitOfWork.ProductRepository.Remove(product);
        //    _unitOfWork.Save();


        //    return Json(new { success = true, message = "Delete Successful" }); 
        //}
        #endregion

    }
}
