using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.SessionSystem;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
    [AdminAuthentication]

    public class MenuController : Controller
	{
		private readonly BonsaiShopContext _context;
		public MenuController(BonsaiShopContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			var listMenu = _context.Menus.ToList();
			ViewBag.ListParentMenu = new SelectList(listMenu.Where(m => m.Levels == 1).ToList(), "MenuId", "MenuName");
            return View(listMenu);
		}

		[HttpPost]
		public IActionResult Create(string MenuName, string Description, long ParrentId)
		{
			try
			{
                if (MenuName.Trim() == "") return Json(new { status = 1 });
                else
                {
                    Menu menu = new Menu();
                    menu.MenuName = MenuName;
                    menu.Description = Description;
                    menu.ParrentId = ParrentId;
                    menu.IsActive = true;
                    menu.Alias = Functions.AliasLink(MenuName);
                    menu.CreatedDate = DateTime.Now;
                    menu.ModifiedDate = DateTime.Now;
                    menu.CreatedBy = HttpContext.Session.GetInt32(SessionKey.USERID);
                    menu.ModifiedBy = HttpContext.Session.GetInt32(SessionKey.USERID);
                    if (ParrentId == 0) menu.Levels = 1;
                    else menu.Levels = 2;

                    _context.Add(menu);
                    _context.SaveChanges();
                    return Json(new
                    {
                        status = 0,
                        message = "Addnew Success"
                    });
                }
            } catch
			{
                return Json(new
                {
                    status = 2,
                    message = "Error From Server"
                });
            }
        }

        [HttpPost]
        public IActionResult Update(string MenuName, string Description, long ParrentId, long MenuId, bool IsActive, long CreatedBy, string CreatedDate )
        {
            try
            {
                if (MenuName.Trim() == "") return Json(new { status = 1 });
                else
                {
                    Menu menu = new Menu();
                    menu.MenuId = MenuId;
                    menu.MenuName = MenuName;
                    menu.Description = Description;
                    menu.ParrentId = ParrentId;
                    menu.IsActive = IsActive;
                    menu.Alias = Functions.AliasLink(MenuName);
                    menu.CreatedDate = DateTime.Parse(CreatedDate);
                    menu.ModifiedDate = DateTime.Now;
                    menu.CreatedBy = CreatedBy;
                    menu.ModifiedBy = HttpContext.Session.GetInt32(SessionKey.USERID);
                    if (ParrentId == 0) menu.Levels = 1;
                    else menu.Levels = 2;

                    _context.Update(menu);
                    _context.SaveChanges();
                    return Json(new
                    {
                        status = 0,
                        menuUpdate = menu,
                        message = "Addnew Success"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    status = 2,
                    message = "Error From Server"
                });
            }
        }

        public IActionResult GetMenuById(long MenuId)
        {
            try
            {
                var menu = _context.Menus.Where(m => m.MenuId == MenuId && m.IsActive == true).FirstOrDefault();
                if(menu != null)
                {
                    return Json(new
                    {
                        status = 0,
                        menuEdit = menu,
                        message = "Get data success"
                    });
                } else
                {
                    return Json(new
                    {
                        status = 1,
                        message = "Get data error"
                    });
                }
                ;
            } catch
            {
                return Json(new
                {
                    status = 2,
                    message = "Error From Server"
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
                var item = await _context.Menus.FindAsync(IdToDelete);
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
                    _context.Menus.Remove(item);
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
