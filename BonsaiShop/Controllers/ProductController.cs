using BonsaiShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace BonsaiShop.Controllers
{
	public class ProductController : Controller
	{
		private readonly BonsaiShopContext _context;
		public ProductController(BonsaiShopContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

		[Route("/san-pham/{slug}-{id:long}")]
		public IActionResult ProductDetail(long id)
		{
			var product = _context.Products.AsNoTracking().Where(m => m.IsDeleted == false && m.IsActive == true && m.ProductId == id).Include(m => m.Category).FirstOrDefault();
			if(product == null)
			{
				return NotFound();
			}
			product.ProductViewCount += 1;
			_context.Products.Update(product);
			_context.SaveChanges();
			return View(product);
		}

		[Route("/danh-muc/{slug}-{id:long}", Name = "ProductByCategory")]
		public IActionResult ProductByCategory(long id, int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 1;

			List<Product> listProduct = new List<Product>();
			listProduct = _context.Products.OrderByDescending(m => m.ProductViewCount).Where(m => m.CategoryId == id && m.IsActive == true && m.IsDeleted == false).ToList();
			PagedList<Product> models = new PagedList<Product>(listProduct.AsQueryable(), pageNumber, pageSize);
			ViewBag.currentPage = pageNumber;
			var category = _context.Categories.Where(m => m.CategoryId == id).FirstOrDefault();
			ViewBag.CategoryName = category.CategoryName;
			return View(models);
		}
	}
}
