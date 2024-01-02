using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonsaiShop.Components
{
	[ViewComponent(Name = "HotBlog")]
	public class HotBlogComponent : ViewComponent
	{
		private readonly BonsaiShopContext _context;
		public HotBlogComponent(BonsaiShopContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync(string location = "home")
		{
			var listBlog = _context.Blogs.AsNoTracking().OrderByDescending(m => m.BlogViewCount).Where(m => m.IsActive == true && m.IsDeleted == false).Include(m => m.Category).Take(3).ToList();
			ViewBag.Location = location;
			return await Task.FromResult((IViewComponentResult)View("Default", listBlog));
		}
	}
}
