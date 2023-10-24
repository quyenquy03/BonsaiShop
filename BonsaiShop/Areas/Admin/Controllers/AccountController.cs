using BonsaiShop.Extension;
using BonsaiShop.Models;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using NuGet.Packaging.Signing;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AccountController : Controller
	{
		private readonly BonsaiShopContext _context;
		public AccountController(BonsaiShopContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
            var listAccount = _context.Users.ToList();
            return View(listAccount);
		}

		public IActionResult Create()
		{
            var roles = _context.AllCodes.OrderByDescending(m => m.Id).Where(m => m.Type == "ROLES").Select(m => m);
            ViewBag.listRoles = new SelectList(roles.ToList(), "Id", "Value");
            return View();
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, Microsoft.AspNetCore.Http.IFormFile? avatar)
		{
			if (user == null)
			{
				return NotFound();
			}
			try
			{
                
                if (ModelState.IsValid)
				{
					if (avatar != null)
					{
						string extension = Path.GetExtension(avatar.FileName);
						string image = Extension.Extensions.ToUrlFriendly(user.FullName) + extension;
						user.Avatar= await Functions.UploadFile(avatar, @"Users", image.ToLower());
						user.Avatar = "Users/" + user.Avatar;
					}
					if (string.IsNullOrEmpty(user.Avatar)) user.Avatar = "avatar-default.jpg";
                    user.IsActive = true;
                    user.Password = HashPassword.MD5Password("123123");
                    _context.Add(user);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
                var roles = _context.AllCodes.OrderByDescending(m => m.Id).Where(m => m.Type == "ROLES").Select(m => m);
                ViewBag.listRoles = new SelectList(roles.ToList(), "Id", "Value", user.RoleId);
                return View(user);
            } catch
			{
				return NotFound();
			}
		}

		public async Task<IActionResult> Edit(long id)
		{
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
			try
			{
				var user = await _context.Users.FindAsync(id);
				if (user == null)
				{
					return NotFound();
				}
				var roles = _context.AllCodes.OrderByDescending(m => m.Id).Where(m => m.Type == "ROLES").Select(m => m);
				ViewBag.listRoles = new SelectList(roles.ToList(), "Id", "Value", user.RoleId);
				return View(user);
			} catch
			{
				return NotFound();
			}
		}
	}
}
