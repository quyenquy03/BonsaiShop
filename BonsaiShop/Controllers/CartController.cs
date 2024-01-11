using AspNetCoreHero.ToastNotification.Abstractions;
using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;

namespace BonsaiShop.Controllers
{
    public class CartController : Controller
    {
        private readonly BonsaiShopContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly INotyfService _notyf;

		public CartController(BonsaiShopContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, INotyfService notyf)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _notyf = notyf;
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
			var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
			var listProvince = _context.Provinces.ToList();
			ViewBag.Provinces = new SelectList(listProvince.ToList(), "ProvinceId", "ProvinceName");
            ViewBag.UserId = UserId;
			return View(GetCartItems());
        }
        [HttpPost]
        [Route("/thanh-toan")]
        public IActionResult CheckOut(string FullName, string Phone, int province, int districts, int commune, string Address)
        {

            if (FullName == null) TempData["FullName"] = "Họ tên không được để trống";
            if (Phone == null) TempData["Phone"] = "Số điện thoại khôgn được để trống";
            if (districts == 0) TempData["districts"] = "Bạn chưa chọn quận - huyện";
            if (commune == 0) TempData["commune"] = "Bạn chưa chọn thị xã";
            if (Address == null) TempData["Address"] = "Địa chỉ không được để trống";

            if (FullName == null || Phone == null || districts == 0 || commune == 0 || Address == null)
            {
				var listProvince = _context.Provinces.ToList();
				ViewBag.Provinces = new SelectList(listProvince.ToList(), "ProvinceId", "ProvinceName");
                return RedirectToAction("Cart");
            }
			var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
            if(UserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
			var feeshipId = 0;
            decimal? feeshipPrice = 20000;
			var feeship = _context.FeeShips.Where(m => m.DistrictId == districts && m.ProvinceId == province && m.CommuneId == commune).FirstOrDefault();
			if (feeship != null)
			{
				feeshipId = Convert.ToInt32(feeship.FeeShipId);
                feeshipPrice = feeship.ShipPrice;
			}
            var provinceName = _context.Provinces.Where(m => m.ProvinceId == province).Select(m => m.ProvinceName).FirstOrDefault();
            var districtName = _context.Districts.Where(m => m.DistrictId == districts).Select(m => m.DistrictName).FirstOrDefault();
            var communeName = _context.Communes.Where(m => m.CommuneId == commune).Select(m => m.CommuneName).FirstOrDefault();
			Random ran = new Random();
			int number = ran.Next(10000);
			var order = new Order();

            order.FullName = FullName;
            order.Address = string.Format("{0} - {1} - {2} - {3}", Address, communeName, districtName, provinceName);
            order.Phone = Phone;
            order.UserId = UserId;
            order.CreatedBy = UserId;
            order.ModifiedBy = UserId;
            order.CreatedDate = DateTime.Now;
            order.ModifiedDate = DateTime.Now;
            order.FeeShipId = feeshipId;
            order.Code = "DONHANG-" + number.ToString();
            
            ViewBag.ShipPrice = feeshipPrice;
            ViewBag.ListCart = GetCartItems();
            ViewBag.ProvinceName = provinceName;
            ViewBag.DistrictName = districtName;
            ViewBag.CommuneName = communeName;
            ViewBag.Address = Address;
			return View(order);
        }
        void SendConfirmMail(Order order)
        {
            var user = _context.Users.Where( m=> m.UserId == order.UserId && m.IsBlocked == 1 && m.IsDeleted == false ).FirstOrDefault();  
            var feeship = _context.FeeShips.Where(m => m.FeeShipId == order.FeeShipId).FirstOrDefault();
            decimal? shipPrice = 25000;
            if(feeship != null )
            {
                shipPrice = feeship.ShipPrice;
            }
            var cart = GetCartItems();
            var content = "";
            foreach(var cartitem in cart)
            {
                var total = String.Format("{0:0,0 đ}", (cartitem.product.ProductPrice - cartitem.product.ProductPrice * cartitem.product.ProductDisCount / 100) * cartitem.quantity);
                var price = String.Format("{0:0,0 đ}", (cartitem.product.ProductPrice - cartitem.product.ProductPrice * cartitem.product.ProductDisCount / 100));
                content += string.Format("<tr> <td style='padding: 5px;'><a href='/'>{1}</a></td> <td style='padding: 5px;'>{2}</td> <td style='padding: 5px;'>{3}</td><td style='padding: 5px;'>{0}</td></tr>",total, cartitem.product.ProductName, price, cartitem.quantity);
            }
            var linkConfirm = _httpContextAccessor?.HttpContext?.Request.Scheme + "://" + _httpContextAccessor?.HttpContext?.Request.Host + "/confirm-buy-product?orderId=" + order.OrderId;
			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587);
				client.Authenticate("ta2k3quyen@gmail.com", "xnxt lspv mynw uenl");
                var bodyBuiler = new BodyBuilder();
				var path = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "EmailTemplate" + Path.DirectorySeparatorChar.ToString() + "ConfirmOrder.html";
				string HtmlBody = "";
				using (StreamReader streamReader = System.IO.File.OpenText(path))
				{
					HtmlBody = streamReader.ReadToEnd();
				}
				string messageBody = string.Format(HtmlBody, 
                    user?.FullName,
                    order.CreatedDate,
                    order.FullName,
                    order.Address,
                    order.Phone,
                    user?.Email,
                    order.CreatedDate,
					content,
					order.TotalAmount,
					shipPrice,
                    order.TotalPayment,
					linkConfirm
					);
				bodyBuiler.HtmlBody = messageBody;
				var message = new MimeMessage
				{
					Body = bodyBuiler.ToMessageBody()
				};

				message.From.Add(new MailboxAddress("Bonsaishop", "ta2k3quyen@gmail.com"));
				message.To.Add(new MailboxAddress(user?.FullName, user?.Email));
				message.Subject = "Xác nhận đặt hàng";
				client.Send(message);
				client.Disconnect(true);
			}
		}

        public IActionResult ConfirmCheckout(Order order, string? province, string? district, string? commune)
        {
            try
            {
				if (order == null)
				{
					return RedirectToAction("CheckOut");
				}

				var cart = GetCartItems();
				if (cart.Count > 0)
				{
                    order.Address = order.Address+", " + commune + ", " + district + ", " + province;
					var newitem = _context.Orders.Add(order);
					_context.SaveChanges();
					var id = newitem.Entity.OrderId;
					foreach (var item in cart)
					{
						var orderItem = new OrderDetail();
						orderItem.ProductId = item.product.ProductId;
						orderItem.Quantity = item.quantity;
						orderItem.Price = item.product.ProductPrice * (100 - item.product.ProductDisCount) / 100;
						orderItem.OrderId = id;
						_context.OrderDetails.Add(orderItem);
						_context.SaveChanges();
					}
					SendConfirmMail(order);
					ClearCart();
				}
                _notyf.Success("Đã đặt hàng thành công, kiểm tra email để xác nhận");
				return RedirectToAction("Index", "Order");
			} catch
            {
                _notyf.Error("Website đang bị lỗi, vui lòng chờ");
				return RedirectToAction("Cart");
			}
		}
    }
}
