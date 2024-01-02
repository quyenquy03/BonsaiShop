using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonsaiShop.Controllers
{
	public class OrderController : Controller
	{
		private readonly BonsaiShopContext _context;
		private readonly INotyfService _notyf;
		public OrderController(BonsaiShopContext context, INotyfService notyf)
		{
			_context = context;
			_notyf = notyf;
		}

		[CustomerAuthentication]
		[Route("/don-hang")]
		public IActionResult Index()
		{
			var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
			if(UserId == null)
			{
				return RedirectToAction("Login", "Account");
			}
			var MyOrder = _context.Orders.Where(m => m.UserId == UserId).ToList() ;
			return View(MyOrder);
		}
		[Route("/confirm-buy-product")]
		public IActionResult ConfirmBuyProduct(int orderId)
		{
			var order = _context.Orders.Where( m => m.OrderId== orderId && m.OrderStatus == 1).FirstOrDefault();
			if(order == null)
			{
				return NotFound();
			}
			order.OrderStatus= 2;
			_context.Orders.Update(order);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
		public IActionResult OrderDetail(int orderId)
		{
			if (orderId == null)
			{
				return NotFound();
			}
			var order = _context.Orders.AsNoTracking().Include(m => m.User).FirstOrDefault(m => m.OrderId == orderId);
			var orderDetail = _context.OrderDetails.AsNoTracking().Where(m => m.OrderId == orderId).Include(m => m.Product).ToList();
			ViewBag.OrderDetail = orderDetail;
			return View(order);
		}
		public IActionResult ChangeOrderStatus(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var itemById = _context.Orders.FirstOrDefault(m => m.OrderId == id);
			if (itemById == null)
			{
				_notyf.Warning("Không thể thực hiện");
				return RedirectToAction("Index");
			}
			itemById.OrderStatus = 5;
			_context.Orders.Update(itemById);
			_context.SaveChanges();
			_notyf.Success("Đã hủy đơn hàng");
			return RedirectToAction("Index");
		}
	}
}
