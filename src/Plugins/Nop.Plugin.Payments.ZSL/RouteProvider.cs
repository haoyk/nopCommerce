using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.ZSL
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //PDT
            routes.MapRoute("Plugin.Payments.ZSL.PDTHandler",
                 "Plugins/PaymentZSL/PDTHandler",
                 new { controller = "PaymentZSL", action = "PDTHandler" },
                 new[] { "Nop.Plugin.Payments.ZSL.Controllers" }
            );
            //IPN
            routes.MapRoute("Plugin.Payments.ZSL.IPNHandler",
                 "Plugins/PaymentZSL/IPNHandler",
                 new { controller = "PaymentZSL", action = "IPNHandler" },
                 new[] { "Nop.Plugin.Payments.ZSL.Controllers" }
            );
            //Cancel
            routes.MapRoute("Plugin.Payments.ZSL.CancelOrder",
                 "Plugins/PaymentZSL/CancelOrder",
                 new { controller = "PaymentZSL", action = "CancelOrder" },
                 new[] { "Nop.Plugin.Payments.ZSL.Controllers" }
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
