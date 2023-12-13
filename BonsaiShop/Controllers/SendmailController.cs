using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using MimeKit;

namespace BonsaiShop.Controllers
{
	public class SendmailController : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		public SendmailController(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			using(var client = new SmtpClient())
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
				string messageBody = string.Format(HtmlBody, "Nguyễn Văn An");
				bodyBuiler.HtmlBody = messageBody;
				var message = new MimeMessage
				{
					Body = bodyBuiler.ToMessageBody()
				};

				message.From.Add(new MailboxAddress("Bonsaishop", "ta2k3quyen@gmail.com"));
				message.To.Add(new MailboxAddress("test", "khuyenca111@gmail.com"));
				message.Subject = "test mail";
				client.Send(message);
				client.Disconnect(true);
			}
			return View();
		}
	}
}
