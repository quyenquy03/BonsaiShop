using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Components
{
	[ViewComponent(Name = "HeaderComponent")]
	public class HeaderComponent : ViewComponent
	{
		private readonly BonsaiShopContext _context;
		public HeaderComponent(BonsaiShopContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var FullName = HttpContext.Session.GetString(SessionKey.FULLNAME);
			ViewBag.FullName = FullName;
			return await Task.FromResult((IViewComponentResult)View("Default"));
		}
	}
}
