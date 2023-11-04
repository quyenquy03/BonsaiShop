using BonsaiShop.Extension;
using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

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
		[AdminAuthentication]
		public IActionResult Index(int? page, int? RoleID = 0, int? ActiveStatus = 0)
		{
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 5;
			
			List<User> listUser = new List<User>();
			if(RoleID == 0 && ActiveStatus == 0)
			{
				listUser = _context.Users.AsNoTracking().OrderByDescending(x => x.UserId).Include(u => u.IsActiveNavigation).Include(m => m.Role).ToList();
			} else
			{
				if(RoleID == 0 && ActiveStatus != 0)
				{
					 listUser = _context.Users.AsNoTracking().OrderByDescending(x => x.UserId).Where(m => m.IsActive == ActiveStatus).Include(u => u.IsActiveNavigation).Include(m=> m.Role).ToList();
				}
				if(RoleID != 0 && ActiveStatus == 0)
				{
                    listUser = _context.Users.AsNoTracking().OrderByDescending(x => x.UserId).Where(m => m.RoleId == RoleID).Include(u => u.IsActiveNavigation).Include(m => m.Role).ToList();
                }
				if(RoleID != 0 && ActiveStatus != 0)
				{
                    listUser = _context.Users.AsNoTracking().OrderByDescending(x => x.UserId).Where(m => m.RoleId == RoleID && m.IsActive == ActiveStatus).Include(u => u.IsActiveNavigation).Include(m => m.Role).ToList();
                }
            }
			
            PagedList<User> models = new PagedList<User>(listUser.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;

            var roles = _context.AllCodes.OrderByDescending(m => m.Id).Where(m => m.Type == "ROLES").Select(m => m);
            ViewBag.listRoles = new SelectList(roles.ToList(), "Id", "Value", RoleID);

            var actives = _context.AllCodes.OrderByDescending(m => m.Id).Where(m => m.Type == "ACTIVE").Select(m => m);
            ViewBag.listActiveStatus = new SelectList(actives.ToList(), "Id", "Value", ActiveStatus);

            ViewBag.RoleID = RoleID;
            return View(models);
		}
		public IActionResult Filter(int? RoleID = 0, int ActiveStatus = 0)
		{
			var url = $"/Admin/Account?RoleID={RoleID}&ActiveStatus={ActiveStatus}";
			if(RoleID == 0)
			{
                url = $"/Admin/Account?ActiveStatus={ActiveStatus}";
            }
            if (ActiveStatus == 0)
            {
                url = $"/Admin/Account?RoleID={RoleID}";
            }
            if (RoleID == 0 && ActiveStatus == 0)
			{
				url = "/Admin/Account/";
			}
            return Json(new
            {
                status = 1,
                LinkURL = url
            });
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
                    user.IsActive = 5;
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

		[HttpPost]
		public async Task<IActionResult> Edit(User user, string OldAvatar, Microsoft.AspNetCore.Http.IFormFile? avatar)
		{
			if(user == null	)
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
                        user.Avatar = await Functions.UploadFile(avatar, @"Users", image.ToLower());
                        user.Avatar = "Users/" + user.Avatar;
                    } else
					{
						user.Avatar = OldAvatar;
					}
					_context.Update(user);
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
		public async Task<IActionResult> Delete(long UserID)
		{
			if(UserID == null)
			{
                return new JsonResult(new
                {
                    message = "Success",
                    status = 0
                });
            }
			try
			{
                var user = await _context.Users.FindAsync(UserID);
				if(user == null)
				{
					return new JsonResult(new {
						message = "Error",
						status = 1
					});
				} else
				{
					_context.Remove(user);
					_context.SaveChanges();
					return new JsonResult(new
					{
						message = "Success",
						status = 0
					});
				}
            } catch
			{
				return new JsonResult(new
				{
					message = "Error",
					status = 1
				});
			}
		}

		public async Task<IActionResult> Block(long UserID)
		{
            if (UserID == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var user = await _context.Users.FindAsync(UserID);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        message = "Error",
                        status = 1
                    });
                }
                else
                {
					if (user.IsActive == 5) user.IsActive = 6;
					else user.IsActive = 5;

                    _context.Update(user);
                    _context.SaveChanges();
					if(user.IsActive == 5)
					{
                        return new JsonResult(new
                        {
                            message = "Unblock Acc Success",
                            status = 0
                        });
                    } else
					{
                        return new JsonResult(new
                        {
                            message = "Block Acc Success",
                            status = 0
                        });
                    }
                    
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
        }
	}
}
