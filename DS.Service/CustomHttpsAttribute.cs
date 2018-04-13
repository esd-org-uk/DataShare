

namespace DS.Service
{
    using System.Web.Mvc;

    public class CustomHttpsAttribute : RequireHttpsAttribute, IAuthorizationFilter
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var useSSLForAdmin = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["UseSSLForAdmin"]??"false");
            if (useSSLForAdmin != true)
            {
                return;
            }
            base.OnAuthorization(filterContext);
        }
    }
}
