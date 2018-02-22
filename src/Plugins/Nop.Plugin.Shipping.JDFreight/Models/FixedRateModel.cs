using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.JDFreight.Models
{
    public class FixedRateModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.JDFreight.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.JDFreight.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}