using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonsaiShop.Components
{
	[ViewComponent(Name = "ProductCategory")]
	public class ProductCategoryComponent : ViewComponent
	{
		private readonly BonsaiShopContext _context;
		public ProductCategoryComponent(BonsaiShopContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var listCate = _context.Categories.AsNoTracking().OrderByDescending(m => m.CategoryId).Where(m => m.CategoryType == 2 && m.IsActive == true).ToList();
			return await Task.FromResult((IViewComponentResult)View("Default", listCate));
		}
	}
}
