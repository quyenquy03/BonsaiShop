using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PagedList.Core;
using System.Globalization;

namespace BonsaiShop.Controllers
{
	public class ProductController : Controller
	{
		private readonly BonsaiShopContext _context;
		public ProductController(BonsaiShopContext context)
		{
			_context = context;
		}

		public const string RECENTPRODUCT = "RecentProduct";
		public const string FAVOURITEPRODUCT = "FavouriteProduct";

		public IActionResult Index()
		{
			return View();
		}

		List<Product> GetProductFromSession(string SessionKey)
		{
			var session = HttpContext.Session;
			string jsonstring = session.GetString(SessionKey);
			if (jsonstring != null)
			{
				return JsonConvert.DeserializeObject<List<Product>>(jsonstring);
			}
			return new List<Product>();
		}

		void ClearSession(string SessionKey)
		{
			var session = HttpContext.Session;
			session.Remove(SessionKey);
		}

		void SaveSession(List<Product> ls, string SessionKey)
		{
			var session = HttpContext.Session;
			string jsonstring = JsonConvert.SerializeObject(ls);
			session.SetString(SessionKey, jsonstring);
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

			/*var RecentProduct = GetProductFromSession(RECENTPRODUCT);
			var IsExists = RecentProduct.Find(p => p.ProductId == product.ProductId);
			if(IsExists != null)
			{
				RecentProduct.Remove(IsExists);
				RecentProduct.Add(product);
			} else
			{
				if(RecentProduct.Count >= 5)
				{
					RecentProduct.Remove(RecentProduct.FirstOrDefault());
					RecentProduct.Add(product);
				} else
				{
					RecentProduct.Add(product);
				}
			}
			SaveSession(RecentProduct, RECENTPRODUCT);*/

			return View(product);
		}

		[Route("/danh-muc-san-pham/{slug}-{id:long}", Name = "ProductByCategory")]
		public IActionResult ProductByCategory(long id, int? page, string? searchinput)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 9;
			List<Product> listProduct = new List<Product>();

			listProduct = _context.Products.Where(m => m.IsActive == true && m.IsDeleted == false).ToList();
			ViewBag.CategoryName = "Tất cả sản phẩm";
			if( searchinput != null && searchinput?.Trim() != "")
			{
				id = 0;
				listProduct = listProduct.Where(m => m.ProductName.Contains(searchinput)).ToList();
				ViewBag.SearchInput = searchinput;
			}
			if (id != 0)
			{
				listProduct = listProduct.Where(m => m.CategoryId == id).ToList();
				var category = _context.Categories.Where(m => m.CategoryId == id).FirstOrDefault();
				ViewBag.CategoryName = category?.CategoryName;
			}

			PagedList<Product> models = new PagedList<Product>(listProduct.AsQueryable(), pageNumber, pageSize);
			ViewBag.currentPage = pageNumber;
			return View(models);
		}
		public IActionResult AddToFavouritePorduct(int productid)
		{
			try
			{
				var product = _context.Products.Where(p => p.ProductId == productid).FirstOrDefault();
				if (product == null) return Json(new
				{
					status = 1,
					message = "Cannot find this product"
				});
				var listFavouriteProduct = GetProductFromSession(SessionKey.FAVOURITEPRODUCT);
				var IsExists = listFavouriteProduct.Find(p => p.ProductId == product.ProductId);
				var res = "Sản phẩm đã được thêm vào yêu thích";
				if(IsExists != null)
				{
					listFavouriteProduct.Remove(IsExists);
					res = "Đã xóa sản phẩm khỏi danh sách sản phẩm yêu thích";
				}
				listFavouriteProduct.Add(product);
				SaveSession(listFavouriteProduct, SessionKey.FAVOURITEPRODUCT);
				return Json(new
				{
					status = 0,
					message = res
				});
			}
			catch
			{
				return Json(new
				{
					status = 2,
					message = "Error from server"
				});
			}
		}
	}
}
