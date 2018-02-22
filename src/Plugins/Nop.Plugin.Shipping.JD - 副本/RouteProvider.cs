using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.JDFreight
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.JDFreight.Configure1",
                 "Plugins/JDFreight/Configure",
                 new { controller = "JDFreight", action = "Configure", },
                 new[] { "Nop.Plugin.Shipping.JDFreight.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
