using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BonsaiShop.Models.Authentication
{
	public class AdminAuthentication : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.HttpContext.Session.GetString(SessionKey.USERID) == null)
			{
				context.Result = new RedirectToRouteResult(
					new RouteValueDictionary {
						{"Area","" },
						{"Controller","Account" },
						{"Action","Login" }
					}
				);
			} else
			{
				if(context.HttpContext.Session.GetInt32(SessionKey.ROLEID) != 1)
				{
					context.Result = new RedirectToRouteResult(
					new RouteValueDictionary {
						{"Area","" },
						{"Controller","Error" },
						{"Action","ErrorRole" }
					}
				);
				}
			}
		}
	}
}
