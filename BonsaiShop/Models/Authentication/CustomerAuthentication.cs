using BonsaiShop.SessionSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BonsaiShop.Models.Authentication
{
    public class CustomerAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString(SessionKey.USERID) == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        {"Controller","Account" },
                        {"Action","Login" }
                    }
                );
            }
        }
    }
}
