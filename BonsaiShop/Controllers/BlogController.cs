using BonsaiShop.Models;
using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace BonsaiShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly BonsaiShopContext _context;
        public BlogController(BonsaiShopContext context)
        {
            _context = context;
        }
        [Route("/danh-muc-bai-viet/{slug}-{id}")]
        public IActionResult Index(int? page, long? id)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;
            List<Blog> listBlog = new List<Blog>();

            listBlog = _context.Blogs.Where(m => m.IsActive == true && m.IsDeleted == false).ToList();
			ViewBag.CategoryName = "Tất cả bài viết";
			if (id != 0)
            {
                listBlog = listBlog.Where(m => m.CategoryId == id).ToList();
				var category = _context.Categories.Where(m => m.CategoryId == id).FirstOrDefault();
				ViewBag.CategoryName = category?.CategoryName;
			}
            PagedList<Blog> models = new PagedList<Blog>(listBlog.AsQueryable(), pageNumber, pageSize);
            return View(models);
        }
        [Route("/bai-viet/{slug}-{id}")]
        public IActionResult Detail(int? id)
        {
			var userid = HttpContext.Session.GetInt32(SessionKey.USERID);
			if (id == null)
            {
                return NotFound();
            }
            var item = _context.Blogs.Where(m => m.BlogId== id && m.IsDeleted == false && m.IsActive == true).FirstOrDefault();
            if(item == null)
            {
                return NotFound();
            }
			var listOtherBlog = _context.Blogs.AsNoTracking().OrderByDescending(m => m.BlogViewCount).Where(m => m.IsActive == true && m.IsDeleted == false && m.CategoryId == item.CategoryId).Include(m => m.BlogComments).Take(3).ToList();
			ViewBag.ListOtherBlog = listOtherBlog;

			item.BlogViewCount += 1;
            _context.Blogs.Update(item);
            _context.SaveChanges();
			ViewBag.UserId = userid;
			return View(item);
        }
		public IActionResult AddComment(string contentcomment, int? postid, long? userid)
		{
			try
			{
				if (contentcomment == null || postid == null || userid == null)
				{
					return Json(new
					{
						status = 2,
						message = "Bạn phải nhập đầy đủ thông tin",
					});
				}
				var newcomment = new BlogComment();
				newcomment.UserId = userid;
				newcomment.BlogId = postid;
				newcomment.Detail = contentcomment;
				newcomment.Levels = 1;
				newcomment.ParrentId = 0;
				_context.BlogComments.Add(newcomment);
				_context.SaveChanges();
				return Json(new
				{
					status = 0,
					message = "Đã gửi bình luận thành công"
				});
			}
			catch
			{
				return Json(new
				{
					status = 1,
					message = "Server đang bị lỗi"
				});
			}
		}
		public IActionResult LoadComment(int? postid)
		{
			try
			{
				if (postid == null)
				{
					return Json(new
					{
						status = 2,
						message = "Không tìm thấy bài viết",
					});
				}
				var listCommentByPost = _context.BlogComments.AsNoTracking().OrderByDescending(m => m.CommentId).Where(m => m.BlogId == postid).Include(m => m.User).Take(5).ToList();
				var data = "";
				foreach (var item in listCommentByPost)
				{
					data += "<div class='comment-item d-flex mt-2'>";
					data += "<img class='comment-avt' src='/images/" + item?.User?.Avatar + "' />";
					data += "<div class='comment-content-box'>";
					data += "<h4 class='comment-name mb-0'>" + item?.User?.FullName + "</h4>";
					data += "<p class='comment-detail mb-0'>" + item?.Detail + "</p>";
					data += "</div> </div>";
				}
				return Json(new
				{
					status = 0,
					content = data,
					message = "Đã load bình luận thành công"
				});
			}
			catch
			{
				return Json(new
				{
					status = 1,
					message = "Server đang bị lỗi"
				});
			}
		}
	}
}
