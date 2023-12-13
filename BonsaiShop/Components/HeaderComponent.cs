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
			var listCategory = _context.Categories.ToList();
			var listCategoryPost = listCategory.Where(m => m.IsActive == true && m.CategoryType == 1);
            var listCategoryProduct = listCategory.Where(m => m.IsActive == true && m.CategoryType == 2);

            ViewBag.FullName = FullName;
			ViewBag.ListCategoryPost = listCategoryPost;
			ViewBag.ListCategoryProduct = listCategoryProduct;
			return await Task.FromResult((IViewComponentResult)View("Default"));
		}
	}
}
