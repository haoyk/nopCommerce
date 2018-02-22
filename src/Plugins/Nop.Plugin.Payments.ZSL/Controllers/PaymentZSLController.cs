using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.ZSL.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.ZSL.Controllers
{
    public class PaymentZSLController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly ZSLPaymentSettings _ZSLPaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public PaymentZSLController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            ZSLPaymentSettings ZSLPaymentSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._ZSLPaymentSettings = ZSLPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ZSLPaymentSettings = _settingService.LoadSetting<ZSLPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.UseSandbox = ZSLPaymentSettings.UseSandbox;
            model.BusinessEmail = ZSLPaymentSettings.BusinessEmail;
            model.PdtToken = ZSLPaymentSettings.PdtToken;
            model.PdtValidateOrderTotal = ZSLPaymentSettings.PdtValidateOrderTotal;
            model.AdditionalFee = ZSLPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = ZSLPaymentSettings.AdditionalFeePercentage;
            model.PassProductNamesAndTotals = ZSLPaymentSettings.PassProductNamesAndTotals;
            model.EnableIpn = ZSLPaymentSettings.EnableIpn;
            model.IpnUrl = ZSLPaymentSettings.IpnUrl;
            model.AddressOverride = ZSLPaymentSettings.AddressOverride;
            model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = ZSLPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.UseSandbox, storeScope);
                model.BusinessEmail_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.BusinessEmail, storeScope);
                model.PdtToken_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.PdtToken, storeScope);
                model.PdtValidateOrderTotal_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.PdtValidateOrderTotal, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
                model.EnableIpn_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.EnableIpn, storeScope);
                model.IpnUrl_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.IpnUrl, storeScope);
                model.AddressOverride_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.AddressOverride, storeScope);
                model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = _settingService.SettingExists(ZSLPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope);
            }

            return View("~/Plugins/Payments.ZSL/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ZSLPaymentSettings = _settingService.LoadSetting<ZSLPaymentSettings>(storeScope);

            //save settings
            ZSLPaymentSettings.UseSandbox = model.UseSandbox;
            ZSLPaymentSettings.BusinessEmail = model.BusinessEmail;
            ZSLPaymentSettings.PdtToken = model.PdtToken;
            ZSLPaymentSettings.PdtValidateOrderTotal = model.PdtValidateOrderTotal;
            ZSLPaymentSettings.AdditionalFee = model.AdditionalFee;
            ZSLPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            ZSLPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            ZSLPaymentSettings.EnableIpn = model.EnableIpn;
            ZSLPaymentSettings.IpnUrl = model.IpnUrl;
            ZSLPaymentSettings.AddressOverride = model.AddressOverride;
            ZSLPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.BusinessEmail, model.BusinessEmail_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.PdtToken, model.PdtToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.PdtValidateOrderTotal, model.PdtValidateOrderTotal_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.EnableIpn, model.EnableIpn_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.IpnUrl, model.IpnUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.AddressOverride, model.AddressOverride_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(ZSLPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate PayPal rounding
        [ValidateInput(false)]
        public ActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.ZSL.RoundingWarning") }, JsonRequestBehavior.AllowGet);

            return Json(new { Result = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            PaymentInfoModel zslPaymentInfo = new PaymentInfoModel
            {
                PaymentInfos = new List<PaymentInfoModel_Item>()
                {
                    new PaymentInfoModel_Item() {Amount = 981.23m, PaymentCompanyId = 1, PaymentCompanyName = "发薪公司"},
                    new PaymentInfoModel_Item() {Amount = 123.45m, PaymentCompanyId = 2, PaymentCompanyName = "发薪公司2"}
                }
            };

            return View("~/Plugins/Payments.ZSL/Views/PaymentInfo.cshtml", zslPaymentInfo);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            //像这样收值
            //CreditCardType = form["CreditCardType"],
            //CreditCardNumber = form["CardNumber"],
            //CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
            //CreditCardExpireYear = int.Parse(form["ExpireYear"]),
            //CreditCardCvv2 = form["CardCode"]

            //传递给下单页面的值
            int selectPaymentCompanyId = 9;

            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CustomValues.Add("selectPaymentCompanyId", selectPaymentCompanyId);
            return paymentInfo;
        }

        public ActionResult CancelOrder(FormCollection form)
        {
            if (_ZSLPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage)
            {
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                    customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order != null)
                {
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}