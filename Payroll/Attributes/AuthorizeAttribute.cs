using System.Web.Mvc;

namespace Payroll.Attributes
{

    public class DefaultAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var action = filterContext.ActionDescriptor;
            if (action.IsDefined(typeof(OverrideAuthorizeAttribute), true)) return;

            base.OnAuthorization(filterContext);
        }
    }
    public class OverrideAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
    
}