using BonsaiShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Controllers
{
	public class ErrorController : Controller
	{
		private readonly BonsaiShopContext _context;
		public ErrorController(BonsaiShopContext context) 
		{ 
			_context = context; 
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult ErrorRole()
		{
			return View();
		}
	}
}
