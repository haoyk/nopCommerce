using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.JDFreight
{
    public class JDFreightSettings : ISettings
    {
        public bool LimitMethodsToCreated { get; set; }

        public bool ShippingByWeightEnabled { get; set; }
    }
}