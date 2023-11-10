using Microsoft.AspNetCore.Mvc;

namespace BonsaiShop.Areas.Admin.Controllers
{
    public class FileManageController : Controller
    {
        [Area("Admin")]
        [Route("/Admin/file-manager")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
