using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExamPortal.Legacy.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                Response.Write("<pre>" + exception + "</pre>");
            }
        }
    }
}
