using BonsaiShop.Models.Authentication;
using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly BonsaiShopContext _context;
		public OrderController(BonsaiShopContext context)
		{
			_context = context;
		}

		public IActionResult Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 10;
			var ListOrder = _context.Orders.ToList();
			PagedList<Order> models = new PagedList<Order>(ListOrder.AsQueryable(), pageNumber, pageSize);
			ViewBag.currentPage = pageNumber;
			return View(models);
		}
	}
}
