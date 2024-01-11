using BonsaiShop.Models.Authentication;
using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
    [AdminAuthentication]

    public class OrderController : Controller
	{
		private readonly BonsaiShopContext _context;
        private readonly INotyfService _notyf;
		public OrderController(BonsaiShopContext context, INotyfService notyf)
		{
			_context = context;
            _notyf = notyf;
		}

		public IActionResult Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 10;
			var ListOrder = _context.Orders.OrderBy(m => m.OrderStatus).ToList();
			PagedList<Order> models = new PagedList<Order>(ListOrder.AsQueryable(), pageNumber, pageSize);
			ViewBag.currentPage = pageNumber;
			return View(models);
		}
        public IActionResult Details(long? orderId)
        {
			if (orderId == null)
			{
				return NotFound();
            }
			var order = _context.Orders.AsNoTracking().Include(m => m.User).FirstOrDefault(m => m.OrderId == orderId);
			var orderDetail = _context.OrderDetails.AsNoTracking().Where(m => m.OrderId == orderId).Include(m => m.Product).ToList();
			ViewBag.OrderDetail = orderDetail;
			return View(order);
		}
        public async Task<IActionResult> DeletePernament(long? IdToDelete)
        {
            if (IdToDelete == null)
            {
                return new JsonResult(new
                {
                    message = "Can not find id",
                    status = 1
                });
            }
            try
            {
                var item = await _context.Orders.FindAsync(IdToDelete);
                if (item == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find user",
                        status = 1
                    });
                }
                else
                {
                    _context.Orders.Remove(item);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        message = "Success",
                        status = 0
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error from server",
                    status = 1
                });
            }
        }
		public IActionResult ChangeOrderStatus(int? id, string type = "duyet")
		{
			if (id == null)
			{
				return NotFound();
			}
			var itemById = _context.Orders.FirstOrDefault(m => m.OrderId == id);
			if (itemById == null)
			{
				_notyf.Warning("Không thể thực hiện");
				return RedirectToAction("Index");
			}
            if(type == "duyet")
            {
				itemById.OrderStatus = 3;
				_notyf.Success("Đã duyệt đơn hàng");
			} else 
            {
				itemById.OrderStatus = 5;
				_notyf.Success("Đã hủy đơn hàng");
			}
			_context.Orders.Update(itemById);
			_context.SaveChanges();
			
			return RedirectToAction("Index", new {Area = "Admin"});
		}
	}
}
