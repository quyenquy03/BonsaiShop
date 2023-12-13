using BonsaiShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace BonsaiShop.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class FeeShipController : Controller
	{
		private readonly BonsaiShopContext _context;
		public FeeShipController(BonsaiShopContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			var listProvince = _context.Provinces.ToList();
			ViewBag.Provinces = new SelectList(listProvince.ToList(), "ProvinceId", "ProvinceName");
			return View();
		}
		public IActionResult ChooseLocation(int id, string action)
		{
			try
			{
				var data = "";
				if(action == "province")
				{
					var listDistricts = _context.Districts.Where(m => m.ProvinceId == id).ToList();
					if(listDistricts.Count> 0)
					{
						foreach(var item in listDistricts)
						{
							data += "<option value='" + item.DistrictId + "'>" + item.DistrictName + "</option>";
						}
					}
				}
				if(action == "district")
				{
                    var listCommune = _context.Communes.Where(m => m.DistrictId == id).ToList();
                    if (listCommune.Count > 0)
                    {
                        foreach (var item in listCommune)
                        {
                            data += "<option value='" + item.CommuneId + "'>" + item.CommuneName + "</option>";
                        }
                    }
                }
                return Json(new
                {
					status = 0,
					message = "success",
					content = data
                });
            } catch
			{
                return Json(new
                {
					status = 1,
					message = "error from server"
                });
            }
		}
		public IActionResult AddFeeship(int provinceid, int districtsid, int communeid, int shipprice)
		{
			try
			{
				if(districtsid == 0 || communeid == 0 || shipprice == null)
				{
					return Json(new
					{
						status = 1,
						message = "Invalid form"
					});
				}

				var IsExists = _context.FeeShips.Where(m => m.ProvinceId == provinceid && m.DistrictId == districtsid && m.CommuneId== communeid).FirstOrDefault();
				if(IsExists == null)
				{
                    var feeship = new FeeShip();
                    feeship.ProvinceId = provinceid;
                    feeship.DistrictId = districtsid;
                    feeship.ShipPrice = shipprice;
                    feeship.CommuneId = communeid;
                    _context.FeeShips.Add(feeship);
                    _context.SaveChanges();
					return Json(new
					{
						status = 0,
						message = "Đã thêm mới phí vận chuyển"
					}) ;
                } else
				{
					IsExists.ShipPrice = shipprice;
                    _context.FeeShips.Update(IsExists);
                    _context.SaveChanges();
                    return Json(new
                    {
                        status = 0,
                        message = "Đã cập nhật phí vận chuyển"
                    });
                }
				
            } catch
			{
                return Json(new
                {
                    status = 2,
                    message = "Error from server"
                });
            }
		}
		public IActionResult LoadFeeship()
		{
            try
            {
                var data = "";
                var listFeeship = _context.FeeShips.AsNoTracking().Include(m => m.Province).Include(m => m.Commune).Include(m => m.District).ToList();
                if (listFeeship.Count > 0)
                {
                    var i =1;
                    foreach (var item in listFeeship)
                    {
                        data += string.Format("<tr> <td>{0}</td> <td>{1}</td> <td>{2}</td> <td>{3}</td>  <td> <input style='width: 100px' value='{4}' class='shipprice-input' /></td> <td> <button class='btn px-2 text-white btn-danger'> <i class='tf-icons bx bx-trash'></i> </button> </td> </tr>", i++, item.Province.ProvinceName, item.District.DistrictName, item.Commune.CommuneName, item.ShipPrice);
                    }
                }
                return Json(new
                {
                    status = 0,
                    message = "success",
                    content = data
                });
            }
            catch
            {
                return Json(new
                {
                    status = 1,
                    message = "error from server"
                });
            }
        }
    }
}
