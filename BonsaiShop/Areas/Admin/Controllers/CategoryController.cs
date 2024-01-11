using BonsaiShop.Extension;
using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PagedList.Core;

namespace BonsaiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthentication]

    public class CategoryController : Controller
    {
        private readonly BonsaiShopContext _context;
        public CategoryController(BonsaiShopContext context)
        {
            _context = context;
        }
        public class CategoryType
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListCatePost()
        {
            var listCatePost = _context.Categories.AsNoTracking().Where(m => m.CategoryType == 1).ToList();
            return View(listCatePost);
        }
        public IActionResult ListCatePro()
        {
            var listCatePost = _context.Categories.AsNoTracking().Where(m => m.CategoryType == 2).ToList();
            return View(listCatePost);
        }
        public async Task<IActionResult> CreateNewCate()
        {
            var ParentCategories = await _context.Categories.Where(m => m.IsActive == true && m.Levels == 1).ToListAsync();
            ViewBag.ListParentCategories = new SelectList(ParentCategories, "CategoryId", "CategoryName");
            
            var listType = new List<CategoryType>();
            listType.Add(new CategoryType { Id = 1, Name = "Danh mục bài viết" });
            listType.Add(new CategoryType { Id = 2, Name = "Danh mục sản phẩm" });
            ViewBag.ListTypeCategory = new SelectList(listType.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNewCate(Category cate)
        {

            if (cate == null)
            {
                return NotFound();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    cate.Alias = Functions.AliasLink(cate.CategoryName);
                    DateTime currentDate = new DateTime();
                    cate.IsActive = true;
                    if (cate.ParentCateId != 0)
                    {
                        cate.Levels = 2;
                    }
                    else
                    {
                        cate.Levels = 1;
                    }
                    _context.Categories.Add(cate);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(ListCatePost));
                }
                var ParentCategories = _context.Categories.Where(m => m.IsActive == true && m.Levels == 1).ToList();
                ViewBag.ListParentCategories = new SelectList(ParentCategories, "CategoryId", "CategoryName");
                
                var listType = new List<CategoryType>();
                listType.Add(new CategoryType { Id = 1, Name = "Danh mục bài viết" });
                listType.Add(new CategoryType { Id = 2, Name = "Danh mục sản phẩm" });
                ViewBag.ListTypeCategory = new SelectList(listType.ToList(), "Id", "Name");
                
                return View("CreateNewCate", cate);
            }
            catch
            {
                return NotFound();
            }
        }
        public IActionResult LoadCateParent(int CateTypeID, int? ParentCateID)
        {
            var listCateParent = _context.Categories.Where(m => m.IsActive == true && m.CategoryType == CateTypeID && m.Levels == 1).ToList();
            string content = "";
            if (ParentCateID == 0)
            {
                content += string.Format("<option value ='0' selected='true'>Không có danh mục cha</ option >");
            }
            else
            {
                content += string.Format("<option value ='0'>Không có danh mục cha</ option >");
            }

            foreach (var item in listCateParent)
            {
                if (ParentCateID == item.CategoryId)
                {
                    content += string.Format("<option value ='{0}'  selected='true'>{1}</ option >", item.CategoryId, item.CategoryName);
                }
                else
                {
                    content += string.Format("<option value ='{0}'>{1}</ option >", item.CategoryId, item.CategoryName);
                }

            }
            return Json(new
            {
                status = 0,
                Content = content,
            });
        }

        public async Task<IActionResult> EditCate(long? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            try
            {
                var CateByID = _context.Categories.Where(m => m.CategoryId == id).First();
                if (CateByID == null)
                {
                    return NotFound();
                }
                var ParentCategories = await _context.Categories.Where(m => m.IsActive == true && m.Levels == 1).ToListAsync();
                ViewBag.ListParentCategories = new SelectList(ParentCategories, "CategoryId", "CategoryName");
                var listType = new List<CategoryType>();
                listType.Add(new CategoryType { Id = 1, Name = "Danh mục bài viết" });
                listType.Add(new CategoryType { Id = 2, Name = "Danh mục sản phẩm" });
                ViewBag.ListTypeCategory = new SelectList(listType.ToList(), "Id", "Name", CateByID.CategoryType);
                return View(CateByID);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCate(Category cate)
        {
            if (cate == null) return NotFound();
            try
            {
                if (ModelState.IsValid)
                {
                    cate.Alias = Functions.AliasLink(cate.CategoryName);
                    if (cate.ParentCateId != 0)
                    {
                        cate.Levels = 2;
                    }
                    else
                    {
                        cate.Levels = 1;
                    }
                    _context.Update(cate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListCatePost));
                }
                var ParentCategories = _context.Categories.Where(m => m.IsActive == true && m.Levels == 1).ToList();
                ViewBag.ListParentCategories = new SelectList(ParentCategories, "CategoryId", "CategoryName");
                var listType = new List<CategoryType>();
                listType.Add(new CategoryType { Id = 1, Name = "Danh mục bài viết" });
                listType.Add(new CategoryType { Id = 2, Name = "Danh mục sản phẩm" });
                ViewBag.ListTypeCategory = new SelectList(listType.ToList(), "Id", "Name", cate.CategoryType);
                return View(cate);
            }
            catch (Exception ex)
            {
                return NotFound();
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
                var ItemById = await _context.Categories.Where(m => m.CategoryId == IdToUpdate).FirstOrDefaultAsync();
                if (ItemById == null) return Json(new
                {
                    status = 2,
                    message = "Cannot find Item"
                });
                ItemById.IsActive = !ItemById.IsActive;
                _context.Categories.Update(ItemById);
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
                var item = await _context.Categories.FindAsync(IdToDelete);
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
                    _context.Categories.Remove(item);
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
