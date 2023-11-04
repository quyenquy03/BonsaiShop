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

    }
}
