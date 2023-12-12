using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Models;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PagedList.Core;

namespace BonsaiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly BonsaiShopContext _context;
        private readonly INotyfService _notyf;
        public PostController(BonsaiShopContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Blog> listBlog = new List<Blog>();
            listBlog = _context.Blogs.AsNoTracking().OrderByDescending(x => x.BlogId).Where(m => m.IsDeleted == false).Include(m => m.Category).ToList();

            PagedList<Blog> models = new PagedList<Blog>(listBlog.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            return View(models);
        }
        public IActionResult GoToTrash(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Blog> listBlog = new List<Blog>();
            listBlog = _context.Blogs.AsNoTracking().OrderByDescending(x => x.BlogId).Where(m => m.IsDeleted == true).Include(m => m.Category).ToList();

            PagedList<Blog> models = new PagedList<Blog>(listBlog.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            return View(models);
        }

        public IActionResult CreatePost()
        {
            var listCategory = _context.Categories.Where(m => m.CategoryType == 1 && m.IsActive == true).ToList();
            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(Blog post, IFormFile? BlogImage)
        {
            if (post == null)
            {
                _notyf.Error("Thêm mới bài viết thất bại");
                return NotFound();
            }
            try
            {
                if (post.BlogName == null) TempData["BlogName"] = "Bạn phải nhập tên bài viết";
                if (post.BlogDesc == null) TempData["BlogDesc"] = "Bạn phải nhập tiêu đề bài viết";
                if (post.CategoryId == 0) TempData["CategoryId"] = "Bạn chưa chọn danh mục bài viết";
                if (post.BlogDetail == null) TempData["BlogDetail"] = "Bạn phải nhập nội dung cho bài viết";

                if (post.BlogName == null ||
                    post.BlogDesc == null ||
                    post.CategoryId == 0 ||
                    post.BlogDetail == null)
                {
                    var listCategory = _context.Categories.Where(m => m.CategoryType == 1 && m.IsActive == true).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", post.CategoryId);
                    _notyf.Error("Thêm mới bài viết thất bại, kiểm tra lại thông tin nhập vào");
                    return View();
                }
                post.BlogSlug = Functions.AliasLink(post.BlogName);
                post.IsActive = true;
                post.IsDeleted = false;
                post.CreatedDate = DateTime.Now;
                post.ModifiedDate = DateTime.Now;
                post.CreatedBy = 1;
                post.ModifiedBy = 1;
                if (BlogImage != null)
                {
                    string extension = Path.GetExtension(BlogImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(post.BlogName) + extension;
                    post.BlogImage = await Functions.UploadFile(BlogImage, @"Blogs", image.ToLower());
                    post.BlogImage = "Blogs/" + post.BlogImage;
                }

                if (string.IsNullOrEmpty(post.BlogImage)) post.BlogImage = "image-default.png";
                _context.Blogs.Add(post);
                await _context.SaveChangesAsync();
                _notyf.Success("Thêm mới bài viết thành công");
                return RedirectToAction("Index");
            }
            catch
            {
                _notyf.Error("Thêm mới bài viết thất bại");
                return NotFound();
            }
        }
        public IActionResult Edit(int id)
        {
            var EditBlogById = _context.Blogs.Where(m => m.BlogId == id && m.IsDeleted == false).FirstOrDefault();
            var listCategory = _context.Categories.Where(m => m.CategoryType == 1 && m.IsActive == true).ToList();
            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", EditBlogById.CategoryId);
            return View(EditBlogById);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Blog post, IFormFile? BlogImage, string OldImage)
        {
            if (post == null)
            {
                return NotFound();
            }
            try
            {
                if(post.BlogName == null) TempData["BlogName"] = "Bạn phải nhập tên bài viết";
                if (post.BlogDesc == null) TempData["BlogDesc"] = "Bạn phải nhập tiêu đề bài viết";
                if (post.CategoryId == 0) TempData["CategoryId"] = "Bạn chưa chọn danh mục bài viết";
                if (post.BlogDetail == null) TempData["BlogDetail"] = "Bạn phải nhập nội dung cho bài viết";

                if (post.BlogName == null ||
                    post.BlogDesc == null ||
                    post.CategoryId == 0 ||
                    post.BlogDetail == null)
                {
                    var listCategory = _context.Categories.Where(m => m.CategoryType == 1 && m.IsActive == true).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", post.CategoryId);
                    _notyf.Error("Cập nhật bài viết thất bại, vui lòng kiểm tra lại thông tin nhập vào");
                    return View();
                }

                post.BlogSlug = Functions.AliasLink(post.BlogName);
                if (BlogImage != null)
                {
                    string extension = Path.GetExtension(BlogImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(post.BlogName) + extension;
                    post.BlogImage = await Functions.UploadFile(BlogImage, @"Blogs", image.ToLower());
                    post.BlogImage = "Blogs/" + post.BlogImage;
                }
                else
                {
                    post.BlogImage = OldImage;
                }

                _context.Blogs.Update(post);
                await _context.SaveChangesAsync();
                _notyf.Success("Cập nhật bài viết thành công");
                return RedirectToAction("Index");
            }
            catch
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> Delete(int IdToDelete)
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
                var ItemById = await _context.Blogs.Where(m => m.IsDeleted == false && m.BlogId == IdToDelete).FirstOrDefaultAsync();
                if (ItemById == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find User",
                        status = 1
                    });
                }
                else
                {
                    ItemById.IsDeleted = true;
                    _context.Update(ItemById);
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
        public async Task<IActionResult> KhoiPhuc(int IdKhoiPhuc)
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
                var khoiphuc = await _context.Blogs.Where(m => m.IsDeleted == true && m.BlogId == IdKhoiPhuc).FirstOrDefaultAsync();
                if (khoiphuc == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find",
                        status = 1
                    });
                }
                else
                {
                    khoiphuc.IsDeleted = false;
                    _context.Update(khoiphuc);
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
        public async Task<IActionResult> UpdateActiveStatus(int IdToUpdate)
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
                var ItemById = await _context.Blogs.Where(m => m.BlogId == IdToUpdate && m.IsDeleted == false).FirstOrDefaultAsync();
                if (ItemById == null) return Json(new
                {
                    status = 2,
                    message = "Cannot find Product"
                });
                ItemById.IsActive = !ItemById.IsActive;
                _context.Blogs.Update(ItemById);
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
    }
}
