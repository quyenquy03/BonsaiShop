using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using static BonsaiShop.Areas.Admin.Controllers.AccountController;

namespace BonsaiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthentication]

    public class ProductController : Controller
    {
        private readonly BonsaiShopContext _context;
        private readonly INotyfService _notyf;
        public ProductController(BonsaiShopContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        public IActionResult Index(int? page, int? CategoryId = 0, string? SearchInput = "")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Product> listProduct = new List<Product>();
            if(CategoryId == 0 && SearchInput == "")
            {
                listProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductId).Where(m =>  m.IsDeleted == false).Include(m => m.Category).ToList();
            } else
            {
                if(CategoryId == 0 && SearchInput != "")
                {
                    listProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductId).Where(m =>  m.IsDeleted == false && m.ProductName.Contains(SearchInput.ToLower())).Include(m => m.Category).ToList();
                } 
                if(SearchInput == "" && CategoryId != 0)
                {
                    listProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductId).Where(m =>  m.IsDeleted == false && m.CategoryId == CategoryId).Include(m => m.Category).ToList();
                } 
                if(SearchInput != "" && CategoryId != 0)
                {
                    listProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductId).Where(m => m.IsDeleted == false && m.CategoryId == CategoryId && m.ProductName.Contains(SearchInput.ToLower())).Include(m => m.Category).ToList();
                }
            }
            PagedList<Product> models = new PagedList<Product>(listProduct.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            var listCategory = _context.Categories.Where(m => m.CategoryType == 2).ToList();

            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", CategoryId);
            ViewBag.SearchInput = SearchInput;
            ViewBag.CategoryId = CategoryId;
            return View(models);
        }
        public IActionResult GoToTrash(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Product> listProduct = new List<Product>();
            listProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductId).Where(m => m.IsActive == true && m.IsDeleted == true).Include(m => m.Category).ToList();
            PagedList<Product> models = new PagedList<Product>(listProduct.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            return View(models);
        }
        public IActionResult Filter(int? CategoryId = 0, string? SearchInput = "")
        {
            var url = $"/Admin/Product?CategoryId={CategoryId}&SearchInput={SearchInput}";
            if(SearchInput == null || SearchInput.Trim() == "")
            {
                if(CategoryId == 0)
                {
                    url = "/Admin/Product";
                } else
                {
                    url = $"/Admin/Product?CategoryId={CategoryId}";
                }
            } else
            {
                if(CategoryId == 0)
                {
                    url = $"/Admin/Product?SearchInput={SearchInput}";
                } else
                {
                    url = $"/Admin/Product?CategoryId={CategoryId}&SearchInput={SearchInput}";
                }
            }
            return Json(new
            {
                status = 0,
                link = url
            });
        }

        public IActionResult CreateProduct()
        {
            var listCategory = _context.Categories.Where(m => m.CategoryType == 2).ToList();
            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName");
            return View();
        }
        public IActionResult EditProduct (long? id)
        {

            if(id == null)
            {
                return NotFound();
            }
            var ProductById = _context.Products.Where( m => m.ProductId== id && m.IsDeleted == false).FirstOrDefault();
            if(ProductById != null)
            {
                var listCategory = _context.Categories.Where(m => m.CategoryType == 2).ToList();
                ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", ProductById.CategoryId);
                return View(ProductById);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile ProductImage)
        {
            if(product == null)
            {
                _notyf.Error("Cannot find your product infomation");
                return NotFound();
            }
            try
            {
                if (product.ProductName == null) TempData["ProductName"] = "Tên sản phẩm không được để trống";
                if (product.ProductPrice == null) TempData["ProductPrice"] = "Giá của sản phẩm không được để trống";
                if (product.ProductDetail == null) TempData["ProductDetail"] = "Bạn phải nhập thông tin chi tiết của sản phẩm";
                if (product.CategoryId == null) TempData["CategoryId"] = "Bạn phải chọn danh mục cho sản phẩm";
                if (product.ProductDesc == null) TempData["ProductDesc"] = "Bạn phải nhập thông tin tóm tắt của sản phẩm";

                if (product.ProductName == null || product.ProductPrice == null || product.ProductDetail == null || product.ProductDesc == null || product.CategoryId == null)
                {
                    var listCategory = _context.Categories.Where(m => m.CategoryType == 2).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", product.CategoryId);
                    _notyf.Error("Thêm sản phẩm thất bại");
                    return View(product);
                }
                if (product.ProductDisCount == null) product.ProductDisCount = 0;
                product.IsActive = true;
                product.IsDeleted = false;
                product.IsBestSeller = false;
                if (ProductImage != null)
                {
                    string extension = Path.GetExtension(ProductImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(product.ProductName) + extension;
                    product.ProductImage = await Functions.UploadFile(ProductImage, @"Products", image.ToLower());
                    product.ProductImage = "Products/" + product.ProductImage;
                }
                if (string.IsNullOrEmpty(product.ProductImage)) product.ProductImage = "image-default.png";
                product.ProductSlug = Functions.AliasLink(product.ProductName);
                product.ProductViewCount = 0;
                product.CreatedBy = 1;
                product.CreatedDate = DateTime.Now;
                product.ModifiedBy = 1;
                product.ModifiedDate = DateTime.Now;

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                _notyf.Success("Thêm mới người dùng thành công!");
                return RedirectToAction("Index");
            } catch
            {
                _notyf.Error("Error From Server");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile ProductImage, string OldProductImage)
        {
            if (product == null)
            {
                _notyf.Error("Cannot find your product infomation");
                return NotFound();
            }
            try
            {
                if (product.ProductName == null) TempData["ProductName"] = "Tên sản phẩm không được để trống";
                if (product.ProductPrice == null) TempData["ProductPrice"] = "Giá của sản phẩm không được để trống";
                if (product.ProductDetail == null) TempData["ProductDetail"] = "Bạn phải nhập thông tin chi tiết của sản phẩm";
                if (product.CategoryId == null) TempData["CategoryId"] = "Bạn phải chọn danh mục cho sản phẩm";
                if (product.ProductDesc == null) TempData["ProductDesc"] = "Bạn phải nhập thông tin tóm tắt của sản phẩm";
                if (product.ProductName == null || product.ProductPrice == null || product.ProductDetail == null || product.ProductDesc == null || product.CategoryId == null)
                {
                    var listCategory = _context.Categories.Where(m => m.CategoryType == 2).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", product.CategoryId);
                    _notyf.Error("Thêm sản phẩm thất bại");
                    return View(product);
                }
                
                if (ProductImage != null)
                {
                    string extension = Path.GetExtension(ProductImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(product.ProductName) + extension;
                    product.ProductImage = await Functions.UploadFile(ProductImage, @"Products", image.ToLower());
                    product.ProductImage = "Products/" + product.ProductImage;
                }
                if (string.IsNullOrEmpty(product.ProductImage)) product.ProductImage = OldProductImage;
                product.ProductSlug = Functions.AliasLink(product.ProductName);
                product.ModifiedBy = 1;
                product.ModifiedDate = DateTime.Now;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _notyf.Success("Thêm mới người dùng thành công!");
                return RedirectToAction("Index");
            }
            catch
            {
                _notyf.Error("Error From Server");
                return NotFound();
            }
        }
        public async Task<IActionResult> Delete(int? IdToDelete)
        {
            if (IdToDelete == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var itemById = await _context.Products.Where(m => m.ProductId == IdToDelete && m.IsDeleted == false).FirstOrDefaultAsync();
                if (itemById == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find User",
                        status = 1
                    });
                }
                else
                {
                    itemById.IsDeleted = true;
                    _context.Update(itemById);
                    _context.SaveChanges();
                    _notyf.Success("Bạn đã xóa sản phẩm thành công");
                    return new JsonResult(new
                    {
                        message = "Success",
                        status = 0
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error from server",
                    status = 1
                });
            }
        }

        public async Task<IActionResult> KhoiPhuc(int? IdKhoiPhuc)
        {
            if (IdKhoiPhuc == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var itemById = await _context.Products.Where(m => m.ProductId == IdKhoiPhuc && m.IsDeleted == true).FirstOrDefaultAsync();
                if (itemById == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find User",
                        status = 1
                    });
                }
                else
                {
                    itemById.IsDeleted = false;
                    _context.Update(itemById);
                    _context.SaveChanges();
                    _notyf.Success("Bạn đã khôi phục sản phẩm thành công");
                    return new JsonResult(new
                    {
                        message = "Success",
                        status = 0
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error from server",
                    status = 1
                });
            }
        }

        public async Task<IActionResult> UpdateBestSeller(int? IdToUpdate)
        {
            if (IdToUpdate == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var ItemById = await _context.Products.Where(m => m.ProductId == IdToUpdate && m.IsDeleted == false).FirstOrDefaultAsync();
                if (ItemById == null) return Json(new
                {
                    status = 2,
                    message = "Cannot find Product"
                });
                ItemById.IsBestSeller = !ItemById.IsBestSeller;
                _context.Products.Update(ItemById);
                _context.SaveChanges();
                return Json(new
                {
                    status = 0,
                    currentValue = ItemById.IsBestSeller,
                    message = "Success"
                });
            }
            catch
            {
                return Json(new
                {
                    status = 3,
                    message = "Error from server"
                });
            }

        }
        public async Task<IActionResult> UpdateActiveStatus(int? IdToUpdate)
        {
            if (IdToUpdate == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var ItemById = await _context.Products.Where(m => m.ProductId == IdToUpdate && m.IsDeleted == false).FirstOrDefaultAsync();
                if (ItemById == null) return Json(new
                {
                    status = 2,
                    message = "Cannot find Product"
                });
                ItemById.IsActive = !ItemById.IsActive;
                _context.Products.Update(ItemById);
                _context.SaveChanges();
                return Json(new
                {
                    status = 0,
                    currentValue = ItemById.IsActive,
                    message = "Success"
                });
            }
            catch
            {
                return Json(new
                {
                    status = 3,
                    message = "Error from server"
                });
            }

        }
        public async Task<IActionResult> DeletePernament(long? IdToDelete)
        {
            if (IdToDelete == null)
            {
                return new JsonResult(new
                {
                    message = "Can not find id",
                    status = 1
                });
            }
            try
            {
                var item = await _context.Products.FindAsync(IdToDelete);
                if (item == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find user",
                        status = 1
                    });
                }
                else
                {
                    _context.Products.Remove(item);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        message = "Success",
                        status = 0
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error from server",
                    status = 1
                });
            }
        }
    }
}
