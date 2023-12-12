using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonsaiShop.Components
{
	[ViewComponent(Name = "NewProduct")]
	public class NewProductComponent : ViewComponent
	{
		private readonly BonsaiShopContext _context;
		public NewProductComponent(BonsaiShopContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var listNewProduct = _context.Products.AsNoTracking().OrderByDescending(m => m.ProductViewCount).Where(m => m.IsActive == true && m.IsDeleted == false).Include(m => m.Category).Take(8).ToList();
			return await Task.FromResult((IViewComponentResult)View("Default", listNewProduct));
		}
	}
}
