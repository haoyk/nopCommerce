using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.ZSL.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.BusinessEmail")]
        public string BusinessEmail { get; set; }
        public bool BusinessEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.PDTToken")]
        public string PdtToken { get; set; }
        public bool PdtToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.PDTValidateOrderTotal")]
        public bool PdtValidateOrderTotal { get; set; }
        public bool PdtValidateOrderTotal_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.EnableIpn")]
        public bool EnableIpn { get; set; }
        public bool EnableIpn_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.IpnUrl")]
        public string IpnUrl { get; set; }
        public bool IpnUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.AddressOverride")]
        public bool AddressOverride { get; set; }
        public bool AddressOverride_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.ZSL.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage")]
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage { get; set; }
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore { get; set; }
    }
}