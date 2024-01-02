using BonsaiShop.Extension;
using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using BonsaiShop.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly BonsaiShopContext _context;
        public AccountController(BonsaiShopContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if(user == null)
            {
                return NotFound();
            }
            try
            {
                string password = HashPassword.MD5Password(user.Password);
                var CheckLogin = _context.Users.Where(m => m.UserName.ToLower() == user.UserName.ToLower() && m.Password == password).FirstOrDefault();
                if(CheckLogin != null)
                {
                    HttpContext.Session.SetString(SessionKey.FULLNAME, CheckLogin.FullName);
                    HttpContext.Session.SetInt32(SessionKey.USERID, (int)CheckLogin.UserId);
                    HttpContext.Session.SetInt32(SessionKey.ROLEID,(int)CheckLogin.RoleId);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    TempData["LoginError"] = "Tài khoản hoặc mật khẩu không chính xác";
                    return View();
                }
            } catch
            {
                return NotFound();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove(SessionKey.FULLNAME);
            HttpContext.Session.Remove(SessionKey.ROLEID);
			HttpContext.Session.Remove(SessionKey.USERID);
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string AgainPass)
        {
            if(user == null)
            {
                return NotFound();
            }
            try
            {
                var checkUserName = _context.Users.Where(m => m.UserName == user.UserName).FirstOrDefault();
                if (user.UserName == null) { TempData["UserNameRequired"] = "Vui lòng nhập tài khoản của bạn"; }
                if(user.FullName == null) { TempData["FullNameRequired"] = "Vui lòng nhập họ và tên của bạn"; }
                if(user.Password == null) { TempData["PasswordRequired"] = "Vui lòng nhập mật khẩu của bạn"; }
                if(user.Email == null) { TempData["EmailRequired"] = "Vui lòng nhập Email của bạn"; }
                if(AgainPass == null) { TempData["AgainPassRequired"] = "Trường này không được để trống"; }
                if (checkUserName != null) { TempData["UserNameExists"] = "Tài khoản này đã được sử dụng";}
                if(user.Password.Trim() != AgainPass.Trim()) { TempData["AgainPassError"] = "Mật khẩu nhập lại không trùng khớp"; }

                if (user.UserName == null || checkUserName != null || user.FullName == null || user.Password == null || user.Email == null || AgainPass == null || user.Password.Trim() != AgainPass.Trim()) {
                    return View(user);
                }
                user.IsBlocked = 1;
                user.RoleId = 2;
                user.Avatar = "avatar-default.jpg";
                user.Password = HashPassword.MD5Password(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            catch
            {
                return NotFound();
            }
        }

    }
}
