using BonsaiShop.Models;
using BonsaiShop.Models.Authentication;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Controllers
{
	public class OrderController : Controller
	{
		private readonly BonsaiShopContext _context;
		public OrderController(BonsaiShopContext context)
		{
			_context = context;
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
	}
}
