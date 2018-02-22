using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.JD;
using Nop.Core.Domain.Logging;
using Nop.Core.JD;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Http;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;
using Nop.Services.JD.Model;
using Nop.Services.Logging;

namespace Nop.Services.JD
{
    public class JDService : IJDService
    {
        private readonly IRepository<JDAddr> _jdAddrRep;
        private readonly IRepository<JDCommonJson> _jdCommonJsonRep;
        private readonly IRepository<JDCategory> _jdCategoryRep;
        private readonly IRepository<JDClientInfo> _jdClientInfoRep;
        private readonly IRepository<JDClientInfos_PaymentCompany> _jdClient_PayComReq;
        private readonly IHttpService _http;
        private readonly ILogger _log;
        private readonly ISettingService _settingService;

        public JDService(IRepository<JDAddr> jdAddrRep,
            IRepository<JDCommonJson> jdCommonJsonRep,
            IRepository<JDCategory> jdCategoryRep,
            IRepository<JDClientInfo> jdClientInfoRep,
            IRepository<JDClientInfos_PaymentCompany> jdClient_PayComReq,
            IHttpService http,
            ILogger log,
            ISettingService settingService
            )
        {
            _jdAddrRep = jdAddrRep;
            _jdCommonJsonRep = jdCommonJsonRep;
            _http = http;
            _log = log;
            _settingService = settingService;
            _jdCategoryRep = jdCategoryRep;
            _jdClientInfoRep = jdClientInfoRep;
            _jdClient_PayComReq = jdClient_PayComReq;
        }

        public string CallApi(string url, string inputParams)
        {
            if (url.IsNullOrEmpty())
                return "";

            string guid = Guid.NewGuid().ToString();
            string result = string.Empty;

            _log.InsertLog(LogLevel.Information, $"{url},入参{guid}", inputParams);

            int retryCount = 3;
            while (result.IsNullOrEmpty() && retryCount > 0)
            {
                retryCount--;
                result = _http.Post(url, inputParams);
                _log.InsertLog(LogLevel.Information, $"{url},出参{guid}", result);
            }

            return result;
        }

        #region Token相关

        private JDToken _JdCommonToken;
        public JDToken JDCommonToken
        {
            get
            {
                //获取Token
                if (_JdCommonToken == null)
                {
                    //获取公用Token用的配置信息
                    JDCommonClientSetting jdCommonClientSetting = _settingService.LoadSetting<JDCommonClientSetting>();
                    jdCommonClientSetting.Check();

                    _JdCommonToken = this.GetJDToken(jdCommonClientSetting);
                }

                //快过期刷新Token
                if (_JdCommonToken != null &&
                    (_JdCommonToken.Access_Token_Expires - DateTime.Now).TotalHours <= 2 &&
                    (_JdCommonToken.Refresh_Token_Expires - DateTime.Now).TotalHours > 1
                )
                {
                    //获取公用Token用的配置信息
                    JDCommonClientSetting jdCommonClientSetting = _settingService.LoadSetting<JDCommonClientSetting>();
                    jdCommonClientSetting.Check();

                    _JdCommonToken = this.RefreshToken(
                        _JdCommonToken.Refresh_Token,
                        jdCommonClientSetting.ClientId,
                        jdCommonClientSetting.ClientSecret
                    );
                }

                if (_JdCommonToken == null)
                    throw new NopException("获取京东公用Token失败，请联系管理员");

                return _JdCommonToken;
            }
            set { _JdCommonToken = value; }
        }

        private Dictionary<string, JDToken> _jdTokens = new Dictionary<string, JDToken>();

        public JDToken GetJDToken(JDClientInfo jdClientInfo)
        {
            _jdTokens.TryGetValue(jdClientInfo.ClientId, out JDToken token);

            if (token == null || token.Access_Token_Expires <= DateTime.Now)
            {
                string str = CallApi("https://bizapi.jd.com/oauth2/accessToken",
                    GetFetchTokenParam(jdClientInfo));

                JDTokenResult json = JsonConvert.DeserializeObject<JDTokenResult>(str);

                if (json != null && json.success)
                {
                    token = json.ToJdToken();
                    _jdTokens.Remove(jdClientInfo.ClientId);
                    _jdTokens.Add(jdClientInfo.ClientId, token);
                }
                else
                {
                    _log.InsertLog(LogLevel.Error, "京东-获取Token失败", str);
                }
            }

            //快过期刷新Token
            if (token != null &&
                (token.Access_Token_Expires - DateTime.Now).TotalHours <= 2 &&
                (token.Refresh_Token_Expires - DateTime.Now).TotalHours > 1
            )
            {
                token = this.RefreshToken(token.Refresh_Token, jdClientInfo.ClientId, jdClientInfo.ClientSecret);
                _jdTokens.Remove(jdClientInfo.ClientId);
                _jdTokens.Add(jdClientInfo.ClientId, token);
            }

            if (token == null)
            {
                _jdTokens.Remove(jdClientInfo.ClientId);
            }

            return token;
        }

        public JDToken RefreshToken(string refreshToken, string clientId, string clientSecret)
        {
            JDToken result = null;

            string str = CallApi("https://bizapi.jd.com/oauth2/accessToken",
                $"refresh_token={refreshToken}&client_id={clientId}&client_secret={clientSecret}");

            JDTokenResult json = JsonConvert.DeserializeObject<JDTokenResult>(str);

            if (json != null && json.success)
            {
                result = json.ToJdToken();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-刷新Token失败", str);
            }

            return result;
        }

        public JDToken GetJdToken(int paymentCompanyId)
        {
            var client = _jdClient_PayComReq.TableNoTracking
                .FirstOrDefault(p => p.PaymentComId == paymentCompanyId)?.JDClientInfo;

            (client == null).TrueThrow($"不能找到发薪公司{paymentCompanyId}对应的京东账号信息，请联系管理员");

            return this.GetJDToken(client);
        }

        #endregion

        #region 公用Token

        public void FetchAddr()
        {
            FetchAddrLoop("", 0);
        }

        /// <summary>
        /// 抓取商品详情
        /// </summary>
        /// <returns></returns>
        public JDSkuDetail FetchSkuDetail(long skuId)
        {
            JDSkuDetail result = null;

            string str = CallApi("https://bizapi.jd.com/api/product/getDetail", $"token={this.JDCommonToken.Access_Token}&sku={skuId}&isShow=true");
            var json = JsonConvert.DeserializeObject<JDSkuDetailResult>(str);

            if (json != null && json.success)
            {
                result = json.ToJDSkuDetail();
                result.Json = str;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取商品详情失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取商品上下架状态
        /// </summary>
        /// <returns></returns>
        public bool FetchSkuState(long skuId)
        {
            bool result = false;

            string str = CallApi("https://bizapi.jd.com/api/product/skuState", $"token={JDCommonToken.Access_Token}&sku={skuId}");
            var json = JsonConvert.DeserializeObject<JDSkuStateResult>(str);
            if (json != null && json.success)
            {
                result = json.result.FirstOrDefault(p => p.sku.Equals(skuId)).State;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取商品上下架状态失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取商品图片
        /// </summary>
        /// <param name="skuId">商品编号</param>
        /// <returns></returns>
        public IList<JDSkuImageOut> FetchSkuImage(long skuId)
        {
            var result = new List<JDSkuImageOut>();

            string str = CallApi("https://bizapi.jd.com/api/product/skuImage", $"token={JDCommonToken.Access_Token}&sku={skuId}");
            var json = JsonConvert.DeserializeObject<JDSkuImageResult>(str);

            if (json != null && json.success)
            {
                result = json.result.FirstOrDefault(p => p.Key.Equals(skuId)).Value
                    .Select(p => new JDSkuImageOut()
                    {
                        Path = p.path,
                        IsPrimary = p.isPrimary == 1,
                        OrderSort = Convert.ToInt32(p.orderSort)
                    }).ToList();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-抓取商品图片失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取商品区域购买限制
        /// </summary>
        /// <returns></returns>
        public Dictionary<long, bool> FetchSkuAreaLimit(JDFetchSkuAreaLimitIn areaLimitIn)
        {
            areaLimitIn.NullCheck("areaLimitIn");
            areaLimitIn.skuIds.NullCheck("skuIds");
            (areaLimitIn.skuIds.Count() > 100).TrueThrow("最高支持100个商品");

            var result = new Dictionary<long, bool>();

            string str = CallApi("https://bizapi.jd.com/api/product/checkAreaLimit",
                $"token={JDCommonToken.Access_Token}&skuIds={string.Join(",", areaLimitIn.skuIds)}&province={areaLimitIn.provinceId}&city={areaLimitIn.cityId}&county={areaLimitIn.countyId}&town={areaLimitIn.townId}");

            JDSkuAreaLimitResult json = JsonConvert.DeserializeObject<JDSkuAreaLimitResult>(str);

            if (json != null && json.success)
            {
                result = new Dictionary<long, bool>();

                JsonConvert.DeserializeObject<List<SkuAreaLimitResult_Detail>>(json.result)
                    .ForEach(p =>
                    {
                        result.Add(p.skuId, p.isAreaRestrict);
                    });
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-抓取商品区域购买限制失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取运费
        /// </summary>
        /// <returns></returns>
        public JDFetchFreightOut FetchFreight(JDFetchFreightIn freightIn)
        {
            JDFetchFreightOut result = null;

            freightIn.NullCheck("freightIn");
            (freightIn.Skus.Count > 50).TrueThrow("最多支持50种商品");

            string str = CallApi("https://bizapi.jd.com/api/order/getFreight",
                $"token={JDCommonToken.Access_Token}&sku={JsonConvert.SerializeObject(freightIn.Skus)}&province={freightIn.Province}&city={freightIn.City}&county={freightIn.County}&town={freightIn.Town}&paymentType={freightIn.PaymentType}");

            JDFreightResult json = JsonConvert.DeserializeObject<JDFreightResult>(str);

            if (json != null && json.success)
            {
                result = json.result.ToJDFetchFreightOut();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取运费失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取商品可售信息（返回结果包含是否支持7天无理由退货）
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public JDFetchIsForSaleOut FetchSkuIsForSale(long skuId)
        {
            JDFetchIsForSaleOut result = null;

            string str = CallApi("https://bizapi.jd.com/api/product/check", $"token={JDCommonToken.Access_Token}&skuIds={skuId}");

            JDForSaleResult json = JsonConvert.DeserializeObject<JDForSaleResult>(str);

            if (json != null && json.success)
            {
                result = json.result.FirstOrDefault(p => p.skuId.Equals(skuId)).ToJDFetchIsForSaleOut();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-抓取商品可售信息失败", str);
            }

            return result;
        }

        /// <summary>
        /// 抓取单个商品京东价格
        /// </summary>
        /// <returns></returns>
        public JDFetchPriceOut FetchSkuJDPrice(long skuId)
        {
            return this.FetchSkuJDPrice(new List<long> { skuId }).FirstOrDefault();
        }

        /// <summary>
        /// 抓取多个个商品京东价格
        /// </summary>
        /// <returns></returns>
        public IList<JDFetchPriceOut> FetchSkuJDPrice(IEnumerable<long> skuIds)
        {
            (skuIds.Count() > 50).TrueThrow("最高支持100个商品");
            IList<JDFetchPriceOut> result = null;

            string str = CallApi("https://bizapi.jd.com/api/price/getSellPrice",
                $"token={JDCommonToken.Access_Token}&sku={string.Join(",", skuIds)}");

            JDPriceResult json = JsonConvert.DeserializeObject<JDPriceResult>(str);
            if (json != null && json.success)
            {
                result = json.result.Select(p => new JDFetchPriceOut()
                {
                    SkuId = p.skuId,
                    JDPrice = p.jdPrice,
                    Price = p.price
                }).ToList();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-抓取商品价格失败", str);
            }

            return result;
        }

        /// <summary>
        /// 批量获取最新京东商品价格
        /// </summary>
        public void BatchUpdateSkuJDPrice()
        {
        }

        /// <summary>
        /// 批量获取京东库存
        /// </summary>
        public void BatchUpdateSkuJDStock()
        {
        }

        /// <summary>
        /// 抓取京东全部分类（品类）列表
        /// 抓取前请手动清空分类表
        /// </summary>
        public void FetchJDCategory()
        {
            //页号，从1开始
            int pageNo = 1;
            //页大小，最大值5000；
            int pageSize = 2000;
            //抓取结果条数
            int resultCount = 2000;

            var categories = new List<JDCategoryOut>();
            while (resultCount == pageSize)
            {
                string str = CallApi("https://bizapi.jd.com/api/product/getCategorys",
                    $"token={JDCommonToken.Access_Token}&pageNo={pageNo}&pageSize={pageSize}");
                pageNo++;

                JDCategoryResult json = JsonConvert.DeserializeObject<JDCategoryResult>(str);
                if (json != null && json.success)
                {
                    resultCount = json.result.categorys.Count;

                    categories.AddRange(
                        json.result.categorys
                        .Where(p => p.state == 1)
                        .Select(p => new JDCategoryOut()
                        {
                            CategoryId = p.catId,
                            CategoryClass = p.catClass,
                            Name = p.name,
                            ParentId = p.parentId
                        }));
                }
                else
                {
                    _log.InsertLog(LogLevel.Error, "京东-抓取京东分类失败", str);
                }
            }

            _jdCategoryRep.Insert(categories.Select(p => p.ToJdCategory()));
        }

        /// <summary>
        /// 使用公用Client获取京东推送信息
        /// </summary>
        /// <param name="type">推送类型，支持多个组合，例如 1,2,3。为空时返回全部类型</param>
        /// <returns></returns>
        public JDMessageOut FetchJDMessage(string type)
        {
            return FetchJDMessageByToken(JDCommonToken.Access_Token, type);
        }

        /// <summary>
        /// 删除京东消息
        /// </summary>
        /// <param name="msgId">京东消息编码</param>
        /// <returns></returns>
        public JDBoolOut DeleteJDMessage(string msgId)
        {
            return DeleteJDMessageByToken(JDCommonToken.Access_Token, msgId);
        }

        #endregion

        #region 发薪公司Token

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public decimal FetchBalance(int paymentCompanyId)
        {
            var token = GetJdToken(paymentCompanyId);

            decimal result = 0;

            string str = CallApi("https://bizapi.jd.com/api/price/getBalance", $"token={token.Access_Token}&payType=4");
            JDBalanceResult json = JsonConvert.DeserializeObject<JDBalanceResult>(str);

            if (json != null && json.success)
            {
                result = json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取余额失败", str);
            }
            return result;
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        public JDSubmitOrderOut SubmitOrder(JDSubmitOrderIn order)
        {
            order.NullCheck("order");
            order.Check();

            JDSubmitOrderOut result = null;
            var token = GetJdToken(order.PaymentCompanyId);
            token.NullCheck("token");
            
            string str = CallApi("https://bizapi.jd.com/api/order/submitOrder",
                $"token={token.Access_Token}&{order.ToRequestUrlParam()}");

            JDSubmitOrderResult json = JsonConvert.DeserializeObject<JDSubmitOrderResult>(str);

            if (json != null)
            {
                if (json.success)
                {
                    result = json.ToJDSubmitOrderOut();
                    result.Json = str;
                }
                else
                {
                    result = new JDSubmitOrderOut()
                    {
                        Success = false,
                        ResultCode = json.resultCode,
                        ResultMessage = json.resultMessage
                    };

                    _log.InsertLog(LogLevel.Error, "京东-下单异常", str);
                }
            }

            return result;
        }

        /// <summary>
        /// 确认订单
        /// 订单确认后京东会异步发起支付，不再需要调用发起支付接口了。
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public JDBoolOut ConfirmOrder(int paymentCompanyId, string jdOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDBoolOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/confirmOrder",
                $"token={token.Access_Token}&jdOrderId={jdOrderId}");

            JDBoolResult json = JsonConvert.DeserializeObject<JDBoolResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.success && json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-确认订单异常", str);
            }

            return result;
        }

        /// <summary>
        /// 取消未确认订单
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public JDBoolOut CancelOrder(int paymentCompanyId, string jdOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDBoolOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/cancel",
                $"token={token.Access_Token}&jdOrderId={jdOrderId}");

            JDBoolResult json = JsonConvert.DeserializeObject<JDBoolResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.success && json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-取消未确认订单异常", str);
            }

            return result;
        }

        /// <summary>
        /// 发起支付
        /// 订单确认后京东会异步发起支付，不再需要调用发起支付接口了。
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public JDBoolOut DoPay(int paymentCompanyId, string jdOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDBoolOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/doPay",
                $"token={token.Access_Token}&jdOrderId={jdOrderId}");

            JDBoolResult json = JsonConvert.DeserializeObject<JDBoolResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.success && json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-发起支付异常", str);
            }

            return result;
        }

        /// <summary>
        /// 订单反查
        /// 返回值为京东的订单号
        /// </summary>
        /// <param name="nopOrderId">Nop订单编号</param>
        /// <returns></returns>
        public JDOrderIdOut SearchJDOrderByNopOrderId(int paymentCompanyId, string nopOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            nopOrderId.NullOrEmptyCheck("nopOrderId");

            var jdOrderId = new JDOrderIdOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/selectJdOrderIdByThirdOrder",
                $"token={token.Access_Token}&thirdOrder={nopOrderId}");

            JDStringlResult json = JsonConvert.DeserializeObject<JDStringlResult>(str);
            if (json != null && json.success)
            {
                jdOrderId.Success = json.success;
                jdOrderId.ResultCode = json.resultCode;
                jdOrderId.ResultMessage = json.resultMessage;
                jdOrderId.JDOrderId = json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-订单反查异常", str);
            }

            return jdOrderId;
        }

        /// <summary>
        /// 抓取京东订单详情
        /// 此方法返回集合类型，当京东没有拆单的情况下只返回一个对象，京东拆单时返回多个对象。
        /// 此方法在支付完成后才可调用，否则无法返回东京拆单明细
        /// </summary>
        /// <returns></returns>
        public JDOrderDetailOut FetchJDOrderDetail(int paymentCompanyId, string jdOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDOrderDetailOut();
            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/selectJdOrder",
                $"token={token.Access_Token}&jdOrderId={jdOrderId}");

            if (str.Contains("cOrder"))
            {
                //拆单情况
                JDOrderSplitResult json = JsonConvert.DeserializeObject<JDOrderSplitResult>(str);

                if (json != null)
                {
                    result = json.ToJDOrderDetailOut();
                }
                else
                {
                    _log.InsertLog(LogLevel.Error, "京东-抓取京东订单状态异常", str);
                }
            }
            else
            {
                //不拆单情况
                JDOrderResult json = JsonConvert.DeserializeObject<JDOrderResult>(str);

                if (json != null)
                {
                    result = json.ToJDOrderDetailOut();
                }
                else
                {
                    _log.InsertLog(LogLevel.Error, "京东-抓取京东订单状态异常", str);
                }
            }

            return result;
        }

        /// <summary>
        /// 查询配送信息
        /// 返回值JSON类型
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public JDOrderTrackOut OrderTrack(int paymentCompanyId, string jdOrderId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDOrderTrackOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/order/orderTrack",
                $"token={token.Access_Token}&jdOrderId={jdOrderId}");

            JDOrderTrackResult json = JsonConvert.DeserializeObject<JDOrderTrackResult>(str);
            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Track = json.result.ToString();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-查询配送信息异常", str);
            }

            return result;
        }

        /// <summary>
        /// 根据发并可公司获取京东推送信息
        /// </summary>
        /// <param name="paymentCompanyId"></param>
        /// <param name="type">推送类型，支持多个组合，例如 1,2,3。为空时返回全部类型</param>
        /// <returns></returns>
        public JDMessageOut FetchJDMessage(int paymentCompanyId, string type)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            return FetchJDMessageByToken(token.Access_Token, type);
        }

        /// <summary>
        /// 删除京东消息
        /// </summary>
        /// <param name="msgId">京东消息编码</param>
        /// <returns></returns>
        public JDBoolOut DeleteJDMessage(int paymentCompanyId, string msgId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            JDBoolOut result = DeleteJDMessageByToken(token.Access_Token, msgId);

            return result;
        }

        /// <summary>
        /// 抓取京东商品价格变化
        /// </summary>
        /// <returns></returns>
        public JDPriceChangeOut FetchJDPriceChange()
        {
            JDPriceChangeOut result = new JDPriceChangeOut();

            var message = this.FetchJDMessageByToken(JDCommonToken.Access_Token, ((int)JDMessageType.价格变更).ToString());

            if (message != null && !message.Json.IsNullOrEmpty())
            {
                JDPriceChangeResult changes =
                    JsonConvert.DeserializeObject<JDPriceChangeResult>(message.Json);

                if (changes != null && changes.success)
                {
                    result.Changes = new Dictionary<string, long>();

                    changes?.result?.ForEach(c =>
                    {
                        result.Changes.Add(c.id, c.result.skuId);
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 抓取京东商品上下架状态
        /// </summary>
        /// <returns></returns>
        public JDSkuStateChangeOut FetchJDSkuStateChange()
        {
            JDSkuStateChangeOut result = new JDSkuStateChangeOut();

            var message = this.FetchJDMessageByToken(JDCommonToken.Access_Token, ((int)JDMessageType.商品上下架变更).ToString());

            if (message != null && !message.Json.IsNullOrEmpty())
            {
                JDSkuStateChangeResult changes =
                    JsonConvert.DeserializeObject<JDSkuStateChangeResult>(message.Json);

                result.Changes = new List<JDSkuStateChange_Detail>();

                changes?.result?.ForEach(c =>
                {
                    result.Changes.Add(new JDSkuStateChange_Detail()
                    {
                        MsgId = c.id,
                        Time = c.time,
                        SkuId = c.result.skuId,
                        State = (JDSkuState)c.result.state
                    });
                });
            }

            return result;
        }

        /// <summary>
        /// 抓取京东订单物流状态
        /// </summary>
        /// <returns></returns>
        public JDLogisticsStateOut FetchLogisticsState(int paymentCompanyId)
        {
            JDLogisticsStateOut result = new JDLogisticsStateOut();

            var message = this.FetchJDMessage(paymentCompanyId, ((int)JDMessageType.订单已妥投).ToString());

            if (message != null && !message.Json.IsNullOrEmpty())
            {
                JDLogisticsStateChangeResult changes =
                    JsonConvert.DeserializeObject<JDLogisticsStateChangeResult>(message.Json);

                result.States = new List<JDLogisticsStateOut_Detail>();

                changes?.result?.ForEach(c =>
                {
                    result.States.Add(new JDLogisticsStateOut_Detail()
                    {
                        MessageId = c.id,
                        JDOrderId = c.result.orderId,
                        State = (LogisticsStateEnum)c.result.state
                    });
                });
            }

            return result;
        }

        #endregion

        #region 售后相关

        /// <summary>
        /// 获取商品可提交售后数据
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public JDIntOut AS_GetAvailableNum(int paymentCompanyId, string jdOrderId, long skuId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            (skuId == default(long)).TrueThrow("skuId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDIntOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/afterSale/getAvailableNumberComp",
                $"token={token.Access_Token}&param={{\"jdOrderId\":{jdOrderId},\"skuId\":{skuId}}}");

            JDIntResult json = JsonConvert.DeserializeObject<JDIntResult>(str);
            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                if (json.success)
                    result.Data = json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取商品可提交售后数据异常", str);
            }

            return result;
        }

        /// <summary>
        /// 获取商品支持的服务类型
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public JDCodeNameListOut AS_GetAferSaleType(int paymentCompanyId, string jdOrderId, long skuId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            (skuId == default(long)).TrueThrow("skuId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDCodeNameListOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/afterSale/getCustomerExpectComp",
                $"token={token.Access_Token}&param={{\"jdOrderId\":{jdOrderId},\"skuId\":{skuId}}}");

            JDCodeNameListResult json =
                JsonConvert.DeserializeObject<JDCodeNameListResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;

                result.Data = json.result?.Select(p =>
                    new JDCodeNameListOut_List() { Code = p.code, Name = p.name }).ToList();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取商品支持的服务类型异常", str);
            }

            return result;
        }

        /// <summary>
        /// 获取商品返回京东方式
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public JDCodeNameListOut AS_GetReturnType(int paymentCompanyId, string jdOrderId, long skuId)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            (skuId == default(long)).TrueThrow("skuId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            var result = new JDCodeNameListOut();

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            string str = CallApi("https://bizapi.jd.com/api/afterSale/getWareReturnJdComp",
                $"token={token.Access_Token}&param={{\"jdOrderId\":{jdOrderId},\"skuId\":{skuId}}}");

            JDCodeNameListResult json =
                JsonConvert.DeserializeObject<JDCodeNameListResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;

                result.Data = json.result?.Select(p =>
                    new JDCodeNameListOut_List() { Code = p.code, Name = p.name }).ToList();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-获取商品返回京东方式异常", str);
            }

            return result;
        }

        /// <summary>
        /// 创建售后服务单
        /// 需要该配送单已经妥投。
        /// 需要先调用10.3接口校验订单中某商品是否可以提交售后服务
        /// 需要先调用10.4接口查询支持的服务类型
        /// 需要先调用10.5接口查询支持的商品返回京东方式
        /// </summary>
        /// <returns></returns>
        public JDBoolOut AS_CreateAfterSaleOrder(JDAfterSaleIn afterOrder)
        {
            afterOrder.NullCheck("afterOrder");
            afterOrder.Check();

            var result = new JDBoolOut();

            var token = GetJdToken(afterOrder.PaymentCompanyId);
            token.NullCheck("token");

            string _params = JsonConvert.SerializeObject(afterOrder);

            string str = CallApi("https://bizapi.jd.com/api/afterSale/createAfsApply",
                $"token={token.Access_Token}&param={_params}");

            JDStringlResult json =
                JsonConvert.DeserializeObject<JDStringlResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                //success为服务单保存状态
                result.Data = json.success;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-创建服务单异常", str);
            }

            return result;
        }

        /// <summary>
        /// 分页查询服务单概要信息
        /// 注意:如此订单反复提交过服务单,本方法将返回所有记录,请注意筛选
        /// </summary>
        /// <param name="jdOrderId">京东订单号</param>
        /// <param name="pageSize">每页大小，默认值100</param>
        /// <param name="pageIndex">页码，1代表第一页</param>
        /// <returns></returns>
        public JDAfterSaleOrderSummaryOut AS_GetAferSaleOrderListByJDOrderId
            (int paymentCompanyId, string jdOrderId, int pageSize = 100, int pageIndex = 1)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");
            jdOrderId.NullOrEmptyCheck("jdOrderId");

            if (pageSize <= 0)
                pageSize = 100;
            if (pageIndex < 1)
                pageIndex = 1;

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            var result = new JDAfterSaleOrderSummaryOut();

            string str = CallApi("https://bizapi.jd.com/api/afterSale/getServiceListPage",
                $"token={token.Access_Token}&param={{jdOrderId:{jdOrderId},pageSize:{pageSize},pageIndex:{pageIndex}}}");

            JDASOSummaryResult json = JsonConvert.DeserializeObject<JDASOSummaryResult>(str);
            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.ToOut();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-分页查询服务单概要信息异常", str);
            }

            return result;
        }

        /// <summary>
        /// 查询服务单详细信息
        /// </summary>
        /// <param name="afsServiceId">服务单号</param>
        /// <param name="detailTypeEnum">京东服务状态详细类型,为空时只获取服务单主信息、商品明细以及客户信息</param>
        /// <returns></returns>
        public JDAfterSaleOrderDetailOut AS_GetAferSaleOrderDetail(int paymentCompanyId, int afsServiceId,
            JDAfterSaleOrderDetailTypeEnum?[] detailTypeEnum)
        {
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            var result = new JDAfterSaleOrderDetailOut();

            string detailTypestr;
            if (detailTypeEnum == null)
            {
                detailTypestr = "[]";
            }
            else if (detailTypeEnum.Any(p => p == JDAfterSaleOrderDetailTypeEnum.全部))
            {
                detailTypestr = "[1,2,3,4,5]";
            }
            else
            {
                detailTypestr = JsonConvert.SerializeObject(detailTypeEnum);
            }

            string str = CallApi("https://bizapi.jd.com/api/afterSale/getServiceDetailInfo",
                $"token={token.Access_Token}&param={{afsServiceId:{afsServiceId},appendInfoSteps:{detailTypestr}}}");

            JDASODetailResult json = JsonConvert.DeserializeObject<JDASODetailResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;

                result.Data = json.ToOut();
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-查询服务单详细信息异常", str);
            }

            return result;
        }

        /// <summary>
        /// 取消售后服务单
        /// </summary>
        /// <param name="reason">取消原因</param>
        /// <param name="afterSaleOrderIds">待取消售后服务单号</param>
        /// <returns></returns>
        public JDBoolOut AS_CancelAfterSaleOrder
            (int paymentCompanyId, string reason, params int[] afterSaleOrderIds)
        {
            reason.NullOrEmptyCheck("reason");
            afterSaleOrderIds.NullCheck("afterSaleOrderIds");
            (afterSaleOrderIds.Any() == false).TrueThrow("待取消售后服务单号不能为空");
            (paymentCompanyId == default(int)).TrueThrow("paymentCompanyId不能为空");

            var token = GetJdToken(paymentCompanyId);
            token.NullCheck("token");

            var result = new JDBoolOut();

            string str = CallApi("https://bizapi.jd.com/api/afterSale/auditCancel",
                $"token={token.Access_Token}&param={{serviceIdList:{JsonConvert.SerializeObject(afterSaleOrderIds)},approveNotes:\"{reason}\"}}");

            JDBoolResult json = JsonConvert.DeserializeObject<JDBoolResult>(str);

            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.success && json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-取消售后服务单异常", str);
            }

            return result;
        }

        #endregion

        #region privateMethods

        private void InsertAddr(JDAddr addr)
        {
            _jdAddrRep.Insert(addr);
        }

        private void InsertCommonJson(JDCommonJson json)
        {
            _jdCommonJsonRep.Insert(json);
        }

        private void FetchAddrLoop(string parent, int addrLevel)
        {
            string url = GetFetchAddrUrlByLevel(addrLevel);
            if (string.IsNullOrEmpty(url))
                return;

            string param = $"token={this.JDCommonToken.Access_Token}";
            if (!string.IsNullOrEmpty(parent))
            {
                param += $"&id={parent}";
            }

            string jsonStr = CallApi(url, param);
            JDAddrResult addr = JsonConvert.DeserializeObject<JDAddrResult>(jsonStr);

            addr.result?.ToList().ForEach(p =>
            {
                _jdAddrRep.Insert(new JDAddr()
                {
                    Code = p.Value,
                    Name = p.Key,
                    Parent = string.IsNullOrEmpty(parent) ? 0 : Convert.ToInt32(parent)
                });
                FetchAddrLoop(p.Value.ToString(), addrLevel + 1);
            });
        }

        private string GetFetchAddrUrlByLevel(int addrLevel)
        {
            string url = string.Empty;
            switch (addrLevel)
            {
                case 0:
                    url = "https://bizapi.jd.com/api/area/getProvince";
                    break;
                case 1:
                    url = "https://bizapi.jd.com/api/area/getCity";
                    break;
                case 2:
                    url = "https://bizapi.jd.com/api/area/getCounty";
                    break;
                case 3:
                    url = "https://bizapi.jd.com/api/area/getTown";
                    break;
            }

            return url;
        }

        private string GetFetchTokenParam(JDClientInfo jdClientInfo)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(jdClientInfo.UserPwd, "MD5")
                .ToLower();

            string sign = $"{jdClientInfo.ClientSecret}{now}{jdClientInfo.ClientId}{jdClientInfo.UserName}{pwd}access_token{jdClientInfo.ClientSecret}";
            sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sign, "MD5")
                .ToUpper();

            string param = $"grant_type=access_token&client_id={jdClientInfo.ClientId}&client_secret={jdClientInfo.ClientSecret}&timestamp={now}&username={jdClientInfo.UserName}&password={pwd}&sign={sign}";

            return param;
        }

        private JDMessageOut FetchJDMessageByToken(string token, string type)
        {
            var result = new JDMessageOut();

            string typeStr = string.Empty;
            if (!string.IsNullOrEmpty(type))
            {
                typeStr = $"&type={type}";
            }

            string str = CallApi("https://bizapi.jd.com/api/message/get", $"token={token}{typeStr}");

            result.Json = str;

            return result;
        }

        private JDBoolOut DeleteJDMessageByToken(string token, string msgId)
        {
            token.NullOrEmptyCheck("token");

            JDBoolOut result = new JDBoolOut();

            string str = CallApi("https://bizapi.jd.com/api/message/del",
                $"token={token}&id={msgId}");

            JDBoolResult json = JsonConvert.DeserializeObject<JDBoolResult>(str);
            if (json != null)
            {
                result.Success = json.success;
                result.ResultCode = json.resultCode;
                result.ResultMessage = json.resultMessage;
                result.Data = json.success && json.result;
            }
            else
            {
                _log.InsertLog(LogLevel.Error, "京东-删除京东消息异常", str);
            }

            return result;
        }

        #endregion
    }
}
