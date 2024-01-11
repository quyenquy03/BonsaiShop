using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Extension;
using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.SessionSystem;
using BonsaiShop.Ultilities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace BonsaiShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly BonsaiShopContext _context;
        private readonly INotyfService _notyfService;
        public AccountController(BonsaiShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        public IActionResult GetInfoUser(string username)
        {
            if(username == null)
            {
                return Json(new
                {
                    status = 1,
                    message = "Không tìm thấy tên tài khoản"
                });
            }
            try
            {
                var user = _context.Users.FirstOrDefault(m => m.UserName == username);
                if (user == null)
                {
                    return Json(new
                    {
                        status = 1,
                        message = "Không tìm thấy tài khoản của bạn"
                    });
                }
                return Json(new
                {
                    status = 0,
                    userName = username,
                    email = user.Email,
                    isBlock = user.IsBlocked,
                    fullName = user.FullName,
                    avatar = user.Avatar,
                    message = "Success"
                });
            } catch
            {
                return Json(new
                {
                    status = 1,
                    message = "Lỗi mất rồi, vui lòng liên hệ Admin để xử lý"
                });
            }
        }
       
        public IActionResult ConfirmChangePass(string username)
        {
            if(username == null)
            {
                return NotFound();
            }
            var user = _context.Users.FirstOrDefault(m => m.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            Random rnd = new Random();
            var code = rnd.Next(100000, 999999);
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.Authenticate("ta2k3quyen@gmail.com", "xnxt lspv mynw uenl");
                var bodyBuiler = new BodyBuilder();
                var time = DateTime.Now;
                
                string messageBody = "Đây là email xác nhận đổi mật khẩu. Bạn đã quên mật khẩu và yêu cầu lấy lại mật khẩu vào lúc " + time + " . Mã xác nhận của bạn là " + code.ToString();
                bodyBuiler.HtmlBody = messageBody;
                var message = new MimeMessage
                {
                    Body = bodyBuiler.ToMessageBody()
                };
                message.From.Add(new MailboxAddress("Bonsaishop", "ta2k3quyen@gmail.com"));
                message.To.Add(new MailboxAddress(user?.FullName, user?.Email));
                message.Subject = "Mã xác nhận lấy lại mật khẩu";
                client.Send(message);
                client.Disconnect(true);
            }
            user.ConfirmCode = code.ToString();
            _context.Users.Update(user);
            _context.SaveChanges();
            return View();
        }
        [HttpPost]
        public IActionResult ConfirmChangePass(User user, string password2)
        {
            if (user.Password == null) TempData["password1"] = "Vui lòng nhập mật khẩu mới";
            if (password2 == null) TempData["password2"] = "Vui lòng nhập lại mật khẩu mới";
            if (user.ConfirmCode == null) TempData["confirmcode"] = "Vui lòng nhập mã xác nhận";
            Random rnd = new Random();
            var code = rnd.Next(100000, 999999);
            if (user.Password != password2)
            {
                TempData["password2"] = "Mật khẩu nhập lại không trùng khớp";
            }
            if(password2 == null || user.Password == null || user.ConfirmCode == null || user.Password != password2)
            {
                return View(user);
            }
            var userByUsername = _context.Users.FirstOrDefault(m => m.UserName == user.UserName);
            if(userByUsername == null)
            {
                ViewBag.ErrorMessage = "Không thể tìm thấy người dùng";
                return View(user);
            }
            if(user.ConfirmCode.Trim() != userByUsername?.ConfirmCode?.Trim())
            {
                TempData["ErrorMessage"] = "Mã xác nhận không chính xác";
                return View(user);
            }
            userByUsername.Password = HashPassword.MD5Password(user.Password); ;
            userByUsername.ConfirmCode = code.ToString();
            _context.Users.Update(userByUsername);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        [Route("/trang-ca-nhan")]
        public IActionResult Profile()
        {
            var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
            var user = _context.Users.AsNoTracking().Where(m => m.UserId == Convert.ToInt64(UserId)).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }
        [CustomerAuthentication]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user, IFormFile? avatar, string? OldAvatar)
        {
            var userId = HttpContext.Session.GetInt32(SessionKey.USERID);
            if (user.UserId != Convert.ToInt64(userId))
            {
                _notyfService.Error("Bạn không có quyền thay đổi thông tin của người khác");
                return Redirect("/");
            }

            try
            {
                var userUpdate = _context.Users.Where(m => m.UserId == Convert.ToInt64(userId)).SingleOrDefault();
                if (userUpdate == null) return NotFound();
                if (user.FullName == null) TempData["FullName"] = "Họ và tên khong được để trống";
                if (user.Email == null) TempData["Email"] = "Email không được để trống";
                if (user.Phone == null) TempData["Phone"] = "Điện thoại không được để trống";
                if (user.FullName == null || user.Email == null ||  user.Phone == null)
                {
                    _notyfService.Error("Phải điền đầy đủ thông tin");
                    return View("Profile", user);
                }
                if (avatar != null)
                {
                    string extension = Path.GetExtension(avatar.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(user.FullName) + extension;
                    userUpdate.Avatar = await Functions.UploadFile(avatar, @"Users", image.ToLower());
                    userUpdate.Avatar = "Users/" + userUpdate.Avatar;
                }
                if (string.IsNullOrEmpty(user.Avatar)) user.Avatar = OldAvatar;

                userUpdate.FullName = user.FullName;
                userUpdate.Email = user.Email;
                userUpdate.Phone = user.Phone;
                userUpdate.Birthday = user.Birthday;
                
                _context.Users.Update(userUpdate);
                _context.SaveChanges();
                _notyfService.Success("Cập nhật thông tin thành công");
                return RedirectToAction("Profile");
            }
            catch
            {
                _notyfService.Error("Lỗi mất rồi");
                return View("Profile");
            }
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
                    if(CheckLogin.IsBlocked == 2)
                    {
                        TempData["AccountBlocked"] = "Tài khoản của bạn đã bị khóa, vui lòng liên hệ Admin để mở lại";
                        return View(user);
                    }
                    HttpContext.Session.SetString(SessionKey.FULLNAME, CheckLogin.FullName);
                    HttpContext.Session.SetInt32(SessionKey.USERID, (int)CheckLogin.UserId);
                    HttpContext.Session.SetInt32(SessionKey.ROLEID,(int)CheckLogin.RoleId);
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");
                } else
                {
                    _notyfService.Error("Tài khoản hoặc mật khẩu không chính xác");
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
                if(user?.Password?.Trim() != AgainPass?.Trim()) { TempData["AgainPassError"] = "Mật khẩu nhập lại không trùng khớp"; }

                if (user?.UserName == null || checkUserName != null || user.FullName == null || user.Password == null || user.Email == null || AgainPass == null || user.Password.Trim() != AgainPass.Trim()) {
                    return View(user);
                }
                user.IsBlocked = 1;
                user.RoleId = 2;
                user.Avatar = "avatar-default.jpg";
                user.Password = HashPassword.MD5Password(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                _notyfService.Success("Đăng kí tài khoản thành công");
                return RedirectToAction("Login");
            }
            catch
            {
                _notyfService.Error("Lỗi, vui lòng kiểm tra lại");
                return NotFound();
            }
        }

    }
}
