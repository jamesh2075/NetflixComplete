using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace NetflixComplete.WebApi.Filters
{
    public class TokenAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string token = null;
            IEnumerable<string> headerValues;
            bool isValid = actionContext.ControllerContext.Request.Headers.TryGetValues("NetflixCustomerToken", out headerValues);
            if (isValid)
            {
                token = headerValues.FirstOrDefault();
                if (String.IsNullOrEmpty(token))
                    isValid = false;
            }
            if (!isValid)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized) { ReasonPhrase = "You must authenticate the user before making this request." };
            }
            else
            {
                actionContext.ControllerContext.RouteData.Values.Add("token", token);
            }
            
            base.OnActionExecuting(actionContext);
        }
    }
}