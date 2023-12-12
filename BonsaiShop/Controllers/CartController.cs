using BonsaiShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BonsaiShop.Controllers
{
    public class CartController : Controller
    {
        private readonly BonsaiShopContext _context;
        public CartController(BonsaiShopContext context)
        {
            _context = context;
        }
        public const string CARTKEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        string TotalCart()
        {
            var ls = GetCartItems();
            decimal? total = 0;
            if(ls.Count > 0) { 
                foreach(var item in ls)
                {
                    total += (item.product.ProductPrice - item.product.ProductPrice * item.product.ProductDisCount / 100) * item.quantity;
                }
            }
            return String.Format("{0:0,0 đ}", total);

		}

        public IActionResult Index()
        {
            return View();
        }

        /*[Route("/Cart/addcart/{productid:int}", Name = "addcart")]*/
        public IActionResult AddToCart( int productid)
        {
            try
            {
				var product = _context.Products.Where(p => p.ProductId == productid).FirstOrDefault();
				if (product == null) return Json(new
				{
					status = 1,
					message = "Cannot find this product"
				});

				var cart = GetCartItems();
				var cartitem = cart.Find(p => p.product.ProductId == productid);
				if (cartitem != null)
				{
					cartitem.quantity++;
				}
				else
				{
					cart.Add(new CartItem() { quantity = 1, product = product });
				}
				SaveCartSession(cart);
				return Json(new
				{
					status = 0,
					message = "Add cart success"
				});
			} catch
            {
                return Json(new
                {
                    status = 2,
                    message = "Error from server"
                });
            }
        }

        /// xóa item trong cart
        public IActionResult RemoveCart( int productid)
        {
            try
            {
				var cart = GetCartItems();
				var cartitem = cart.Find(p => p.product.ProductId == productid);
				if (cartitem != null)
				{
					cart.Remove(cartitem);
				}
				SaveCartSession(cart);
                var CartNumber = cart.Count;
                return Json(new
                {
                    status = 0,
                    cartnumber= CartNumber,
                    message = "Remove Item from cart success"
                });
			} catch
            {
                return Json(new
                {
                    status = 1,
                    message = "Error from server"
                });
            }
        }
        public IActionResult DecreaseCount(int productid)
        {
			try
			{
				var cart = GetCartItems();
				var cartitem = cart.Find(p => p.product.ProductId == productid);
				string TotalPrice = "";
				if (cartitem != null)
				{
                    cartitem.quantity -= 1;
					TotalPrice = String.Format("{0:0,0 đ}", (cartitem.product.ProductPrice - cartitem.product.ProductPrice * cartitem.product.ProductDisCount / 100) * cartitem.quantity);
				}
				SaveCartSession(cart);
                string totalCart = TotalCart();
				return Json(new
				{
					status = 0,
					totalprice = TotalPrice,
                    totalcart = totalCart,
					message = "Remove Item from cart success"
				});
			}
			catch
			{
				return Json(new
				{
					status = 1,
					message = "Error from server"
				});
			}
		}
		public IActionResult IncreaseCount(int productid)
		{
			try
			{
				var cart = GetCartItems();
				var cartitem = cart.Find(p => p.product.ProductId == productid);
                string TotalPrice = "";
				if (cartitem != null)
				{
					cartitem.quantity += 1;
					TotalPrice = String.Format("{0:0,0 đ}", (cartitem.product.ProductPrice - cartitem.product.ProductPrice * cartitem.product.ProductDisCount / 100) * cartitem.quantity);
				}
				SaveCartSession(cart);
                string totalCart = TotalCart();
				return Json(new
				{
					status = 0,
					totalprice = TotalPrice,
                    totalcart = totalCart,
					message = "Remove Item from cart success"
				});
			}
			catch
			{
				return Json(new
				{
					status = 1,
					message = "Error from server"
				});
			}
		}

		/// Cập nhật
		[Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }


        // Hiện thị giỏ hàng
        [Route("/gio-hang", Name = "cart")]
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

        [Route("/checkout")]
        public IActionResult CheckOut()
        {
            // Xử lý khi đặt hàng
            return View(GetCartItems());
        }

        /*public bool Order(string name, string phone, string address)
        {
            // Xử lý khi đặt hàng thành công
            try
            {
                var cart = GetCartItems();
                if (cart.IsNullOrEmpty())
                {
                    return false;
                }
                int totalAmount = 0;
                foreach (var item in cart)
                {
                    if (item.product.PriceSale == 0)
                    {
                        totalAmount += item.quantity * (int)item.product.Price;
                    }
                    else
                    {
                        totalAmount += item.quantity * (int)item.product.PriceSale;
                    }
                }
                var order = new TbOrder();
                order.CustomerName = name;
                order.Phone = phone;
                order.Address = address;
                order.TotalAmount = totalAmount;
                order.OrderStatusId = 1;
                order.CreatedDate = DateTime.Now;
                _context.TbOrders.Add(order);
                _context.SaveChanges();
                int orderId = order.OrderId;
                foreach (var item in cart)
                {
                    var orderDetail = new TbOrderDetail();
                    orderDetail.OrderId = orderId;
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Price = item.product.Price;
                    orderDetail.Quantity = item.quantity;
                    _context.TbOrderDetails.Add(orderDetail);
                    _context.SaveChanges();
                }
                ClearCart();
                return true;
            }
            catch
            {
                return false;
            }
        }
*/
        //------------------

        
    }
}
