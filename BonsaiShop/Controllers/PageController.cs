using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonsaiShop.Controllers
{
	public class PageController : Controller
	{
		private readonly BonsaiShopContext _context;
		private readonly INotyfService _otyfService;
		public PageController(BonsaiShopContext context, INotyfService notyfService)
		{
			_context = context;
			_otyfService = notyfService;
		}
		public IActionResult Index()
		{
			return View();
		}
		[Route("/lien-he")]
		public IActionResult Contact()
		{
			return View();
		}
        [Route("/gioi-thieu")]
        public IActionResult Introduce()
		{
			return View();
		}
        [Route("/dich-vu/{slug}")]
        public IActionResult Service()
        {
            return View();
        }
        [HttpPost]
		public IActionResult SendRequest(Contact contact)
		{
			if (contact == null)
			{
				return View();
			}
			try
			{
				var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
				if(UserId == null)
				{
					_otyfService.Error("Bạn phải đăng nhập trước khi phản hồi");
					return RedirectToAction("Login", "Account");
				}
				if (contact.Name == null || contact.Email == null || contact.Phone == null || contact.Message == null)
				{
					_otyfService.Error("Vui lòng nhập đầy đủ thông tin");
					return View("Contact", contact);
				}
				contact.CreatedBy = UserId;
				contact.CreatedDate = DateTime.Now;
				_context.Contacts.Add(contact);
				_context.SaveChanges();
				_otyfService.Success("Bạn đã gửi tin nhắn thành công");
				return RedirectToAction("Contact");
			}
			catch
			{
				return View("Index");
			}
		}
	}
}
