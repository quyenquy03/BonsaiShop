using BonsaiShop.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
    [AdminAuthentication]

    public class HomeController : Controller
	{

		public IActionResult Index()
		{
			return View();
		}
	}
}
