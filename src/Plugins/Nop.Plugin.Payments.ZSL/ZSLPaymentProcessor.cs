using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.ZSL.Controllers;
using Nop.Plugin.Payments.ZSL.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments.ZSL
{
    /// <summary>
    /// ZSL payment processor
    /// </summary>
    public class ZSLPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly HttpContextBase _httpContext;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly ZSLPaymentSettings _ZSLPaymentSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ZSLPaymentProcessor(CurrencySettings currencySettings,
            HttpContextBase httpContext,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IWebHelper webHelper,
            ZSLPaymentSettings ZSLPaymentSettings,
            IOrderProcessingService orderProcessingService,
            ICustomerService customerService,
            IWorkContext workContext)
        {
            this._currencySettings = currencySettings;
            this._httpContext = httpContext;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._ZSLPaymentSettings = ZSLPaymentSettings;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("�Ҳ����ͻ���Ϣ");

            //��֧��ҳ�洫�ݹ�����ֵ
            processPaymentRequest.CustomValues.TryGetValue("selectPaymentCompanyId", out Object selectPaymentCompanyIdObj)
                .FalseThrow("��ȡ֧����Ϣ�쳣");

            //ѡ��ķ�н��˾����
            int selectPaymentCompanyId = (int)selectPaymentCompanyIdObj;

            (selectPaymentCompanyId <= 0).TrueThrow("֧��ʧ�ܣ���н��˾���벻��С��0");

            #region  ֧������
            //���ﳵ��ϸ
            var cart = processPaymentRequest.ShoppingCartItems.ToList();

            #region ���ﳵ�ܽ��

            decimal? shoppingCartTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart);
            if (shoppingCartTotal.HasValue)
            {
                //�ܽ��
                decimal totalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotal.Value, _workContext.WorkingCurrency);
            }

            //selectPaymentCompanyIdΪ��н��˾����
            //totalAmount Ϊ��֧�����
            //processPaymentRequest.OrderGuid Ϊ����Guid����
            //������Ϣ��˳��֧��ʱ���õ�

            #endregion

            #endregion


            result.AuthorizationTransactionResult = "��֧��(���Խ׶�)";
            result.NewPaymentStatus = PaymentStatus.Paid;


            return result;
        }

        /// <summary>
        /// ���󸶿�����(��Ҫ�ض���һ����������֧������ʹ�õ�URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //˳��֧�����̳������������PaymentMethodType=Standard,���Բ�����ת��������ַ֧�����˷����Ͳ��ᱻ����
        }

        /// <summary>
        /// ����ʱ�Ƿ�����֧����ť
        /// </summary>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// ����֧�������շѽ��
        /// </summary>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _ZSLPaymentSettings.AdditionalFee, _ZSLPaymentSettings.AdditionalFeePercentage);
            return result;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("��˳��֧����֧�ָ���");
            return result;
        }

        /// <summary>
        /// �˿�
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            Order order = refundPaymentRequest.Order;
            if (order == null)
            {
                result.AddError("����Ϊ��");
                return result;
            }
            //PaymentInfo paymentInfo = _paymentInfoService.GetByOrderId(order.Id);
            //if (!(paymentInfo != null && !string.IsNullOrEmpty(paymentInfo.Out_Trade_No)))
            //{
            //    result.AddError("���׺�Ϊ��");
            //    return result;
            //}

            if (refundPaymentRequest.AmountToRefund <= 0)
            {
                result.AddError("�˿������0");
                return result;
            }
            //if (refundPaymentRequest.AmountToRefund + refundPaymentRequest.Order.RefundedAmount > paymentInfo.Total)
            //{
            //    result.AddError("�˿������");
            //    return result;
            //}



            //�˿�֪ͨ
            string notify_url = _webHelper.GetStoreLocation(false) + "Plugins/AliPay/RefundNotify";

            //result.AddError("�˿��������ύ,�뵽֧������վ�н����˿�ȷ��");//������,����Ӱ���˿���
            return result;
        }

        /// <summary>
        /// ����֧��
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// ���ڸ���
        /// </summary>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("��֧�ֶ��ڸ���");
            return result;
        }

        /// <summary>
        /// ���ڸ���
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("��֧�ֶ��ڸ���");
            return result;
        }

        /// <summary>
        /// ����������֧��δ�ɹ����Ƿ�֧���ظ�֧��
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        /// <summary>
        /// ����·��
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentZSL";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.ZSL.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// ֧����Ϣ·��
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentZSL";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.ZSL.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentZSLController);
        }

        /// <summary>
        /// ��װ���
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ZSLPaymentSettings
            {
                UseSandbox = true,
                BusinessEmail = "test@test.com",
                PdtToken = "Your PDT token here...",
                PdtValidateOrderTotal = true,
                EnableIpn = true,
                AddressOverride = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AddressOverride", "Address override");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.AddressOverride.Hint", "For people who already have PayPal accounts and whom you already prompted for a shipping address before they choose to pay with PayPal, you can use the entered address instead of the address the person has stored with PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.BusinessEmail", "Business Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.BusinessEmail.Hint", "Specify your PayPal business email.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn", "Enable IPN (Instant Payment Notification)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn.Hint", "Check if IPN is enabled.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn.Hint2", "Leave blank to use the default IPN handler URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.IpnUrl", "IPN Handler");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.IpnUrl.Hint", "Specify IPN Handler.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTToken", "PDT Identity Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTToken.Hint", "Specify PDT identity token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTValidateOrderTotal", "PDT. Validate order total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTValidateOrderTotal.Hint", "Check if PDT handler should validate order totals.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.RedirectionTip", "You will be redirected to PayPal site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage", "Return to order details page");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on PayPal site WITHOUT completing a payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.Instructions", "<p><b>If you're using this gateway ensure that your primary store currency is supported by Paypal.</b><br /><br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br /><br />1. Log in to your PayPal account (click <a href=\"https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8\" target=\"_blank\">here</a> to create your account).<br />2. Click the Profile subtab.<br />3. Click Website Payment Preferences in the Seller Preferences column.<br />4. Under Auto Return for Website Payments, click the On radio button.<br />5. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment (http://www.yourStore.com/Plugins/PaymentZSL/PDTHandler).<br />6. Under Payment Data Transfer, click the On radio button.<br />7. Click Save.<br />8. Click Website Payment Preferences in the Seller Preferences column.<br />9. Scroll down to the Payment Data Transfer section of the page to view your PDT identity token.<br /><br /><b>Two ways to be able to receive IPN messages (optional):</b><br /><br /><b>The first way is to check 'Enable IPN' below.</b> It will include in the request the url of you IPN handler<br /><br /><b>The second way is to confugure your paypal account to activate this service</b>; follow these steps:<br />1. Log in to your Premier or Business account.<br />2. Click the Profile subtab.<br />3. Click Instant Payment Notification in the Selling Preferences column.<br />4. Click the 'Edit IPN Settings' button to update your settings.<br />5. Select 'Receive IPN messages' (Enabled) and enter the URL of your IPN handler (http://www.yourStore.com/Plugins/PaymentZSL/IPNHandler).<br />6. Click Save, and you should get a message that you have successfully activated IPN.</p>");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.PaymentMethodDescription", "You will be redirected to PayPal site to complete the payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.ZSL.RoundingWarning", "It looks like you have \"ShoppingCartSettings.RoundPricesDuringCalculation\" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal only rounds to two decimals.");

            base.Install();
        }

        /// <summary>
        /// ж�ز��
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ZSLPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AddressOverride");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.AddressOverride.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.BusinessEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.BusinessEmail.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.EnableIpn.Hint2");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.IpnUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.IpnUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PassProductNamesAndTotals");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PassProductNamesAndTotals.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTToken");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTValidateOrderTotal");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.PDTValidateOrderTotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.Instructions");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.ZSL.RoundingWarning");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// ֧�ָ���
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// ֧�ֲ����˿�
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return true; }
        }

        /// <summary>
        /// ֧���˿�
        /// </summary>
        public bool SupportRefund
        {
            get { return true; }
        }

        /// <summary>
        /// �Ƿ�֧������֧��
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// ���ڸ��ʽ
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// ֧����ʽ
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
        }

        /// <summary>
        /// �Ƿ���ʾ����ĸ�����Ϣҳ��
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// ��֧����ʽҳ�潫��ʾ��֧����ʽ����
        /// </summary>
        public string PaymentMethodDescription
        {
            get { return _localizationService.GetResource("Plugins.Payments.ZSL.PaymentMethodDescription"); }
        }

        #endregion
    }
}
