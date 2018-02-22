using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.JDFreight.Data;
using Nop.Plugin.Shipping.JDFreight.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.JD;
using Nop.Services.JD.DTO;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.JDFreight
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class JDFreightComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly JDFreightSettings _fixedOrByWeightSettings;
        private readonly IStoreContext _storeContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingByWeightObjectContext _objectContext;
        private readonly IJDService _jdService;
        private readonly ILogger _log;

        #endregion

        #region Ctor

        public JDFreightComputationMethod(ISettingService settingService,
            IShippingService shippingService,
            JDFreightSettings fixedOrByWeightSettings,
            IShippingByWeightService shippingByWeightService,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ShippingByWeightObjectContext objectContext,
            IJDService jdService,
            ILogger log)
        {
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._fixedOrByWeightSettings = fixedOrByWeightSettings;
            this._shippingByWeightService = shippingByWeightService;
            this._storeContext = storeContext;
            this._priceCalculationService = priceCalculationService;
            this._objectContext = objectContext;
            this._jdService = jdService;
            this._log = log;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// 获取运费
        /// </summary>
        /// <param name="shippingMethodId">配送</param>
        /// <param name="shippingMethodName"></param>
        /// <param name="getShippingOptionRequest"></param>
        /// <returns></returns>
        private decimal GetRate(int shippingMethodId, string shippingMethodName, GetShippingOptionRequest getShippingOptionRequest)
        {
            if (shippingMethodName.Contains("京东"))
            {
                return this.GetJdFright(getShippingOptionRequest);
            }
            else
            {
                var key = string.Format("ShippingRateComputationMethod.JD.Rate.ShippingMethodId{0}", shippingMethodId);
                var rate = _settingService.GetSettingByKey<decimal>(key);
                return rate;
            }

        }

        /// <summary>
        /// 获取京东运费
        /// </summary>
        /// <param name="getShippingOptionRequest"></param>
        /// <returns></returns>
        private decimal GetJdFright(GetShippingOptionRequest getShippingOptionRequest)
        {
            try
            {
                var freightObj = new JDFetchFreightIn()
                {
                    Province = getShippingOptionRequest.ShippingAddress.JDAddrLevel1,
                    City = getShippingOptionRequest.ShippingAddress.JDAddrLevel2,
                    County = getShippingOptionRequest.ShippingAddress.JDAddrLevel3,
                    Town = getShippingOptionRequest.ShippingAddress.JDAddrLevel4,
                    Skus = getShippingOptionRequest.Items.Where(p => p.ShoppingCartItem.Product.JDSkuId.HasValue)
                        .Select(p => new JDFetchFreightIn_Skus()
                        {
                            SkuId = p.ShoppingCartItem.Product.JDSkuId.Value,
                            Num = p.ShoppingCartItem.Quantity
                        }).ToList()
                };

                var result = _jdService.FetchFreight(freightObj);

                return result.Freight;
            }
            catch (Exception e)
            {
                //log
                return 0;
            }

        }
        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("没有配送商品");
                return response;
            }

            var restrictByCountryId = getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
            var shippingMethods = _shippingService.GetAllShippingMethods(restrictByCountryId);

            foreach (var shippingMethod in shippingMethods)
            {
                var shippingOption = new ShippingOption
                {
                    Name = shippingMethod.GetLocalized(x => x.Name),
                    Description = shippingMethod.GetLocalized(x => x.Description),
                    Rate = GetRate(shippingMethod.Id, shippingMethod.Name, getShippingOptionRequest)
                };
                response.ShippingOptions.Add(shippingOption);
            }

            return response;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            //暂不支持固定费额运费
            return null;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "JDFreight";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.JDFreight.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new JDFreightSettings
            {
                LimitMethodsToCreated = false,
                ShippingByWeightEnabled = false
            };
            _settingService.SaveSetting(settings);

            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.ShippingByWeight", "By Weight");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fixed", "Fixed Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Warehouse.Hint", "If an asterisk is selected, then this shipping rate will apply to all warehouses.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Country.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.StateProvince.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.ShippingMethod", "Shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.ShippingMethod.Hint", "Choose shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.From", "Order weight from");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.From.Hint", "Order weight from.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.To", "Order weight to");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.To.Hint", "Order weight to.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.AdditionalFixedCost", "Additional fixed cost");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.AdditionalFixedCost.Hint", "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LowerWeightLimit", "Lower weight limit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LowerWeightLimit.Hint", "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.PercentageRateOfSubtotal.Hint", "Charge percentage (of subtotal).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.RatePerWeightUnit", "Rate per weight unit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.RatePerWeightUnit.Hint", "Rate per weight unit.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they've not configured here (zero shipping fee in this case).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.DataHtml", "Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.AddRecord", "Add record");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Formula", "Formula to calculate rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.JDFreight.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<JDFreightSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Warehouse");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Warehouse.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.ShippingMethod");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.ShippingMethod.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.From");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.From.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.To");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.To.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.AdditionalFixedCost");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.AdditionalFixedCost.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LowerWeightLimit");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LowerWeightLimit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.PercentageRateOfSubtotal");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.PercentageRateOfSubtotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.RatePerWeightUnit");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.RatePerWeightUnit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LimitMethodsToCreated");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.LimitMethodsToCreated.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Fields.DataHtml");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Formula");
            this.DeletePluginLocaleResource("Plugins.Shipping.JDFreight.Formula.Value");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Realtime;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get
            {
                //uncomment a line below to return a general shipment tracker (finds an appropriate tracker by tracking number)
                //return new GeneralShipmentTracker(EngineContext.Current.Resolve<ITypeFinder>());
                return null;
            }
        }

        #endregion
    }
}
