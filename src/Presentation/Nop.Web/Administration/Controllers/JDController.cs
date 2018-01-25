using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core.Domain.JD;
using Nop.Services.JD;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;
using Nop.Web.Framework;

namespace Nop.Admin.Controllers
{
    public class JDController : Controller
    {
        private readonly IJDService _jdService;

        public JDController(IJDService jdService)
        {
            _jdService = jdService;
        }

        // GET: JD
        public ActionResult Index()
        {
            //_jdService.InsertAddr(new JDAddr()
            //{
            //    Id = 1,
            //    Name = "name",
            //    Parent = 0
            //});


            HttpPost.Post("https://bizapi.jd.com/api/product/getDetail", "token=eeCuxJ01NGXe5JYFGIM834XK5&sku=635651");

            return Content("");
        }

        public ActionResult FetchAddr()
        {
            _jdService.FetchAddr();

            return Content("OK");
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <returns></returns>
        public JsonResult FetchSku(long id)
        {
            var sku = _jdService.FetchSkuDetail(id);

            return Json(JsonConvert.SerializeObject(sku), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 定时访问该Action以防止公用Token过期
        /// </summary>
        /// <returns></returns>
        public ActionResult KeepCommonTokenAlive()
        {
            var token = _jdService.JDCommonToken;
            return Content("OK");
        }

        /// <summary>
        /// 抓取商品上下架状态
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchSkuState(long id)
        {
            return Content(_jdService.FetchSkuState(id).ToString());
        }

        /// <summary>
        /// 抓取商品图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FetchSkuImage(long id)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchSkuImage(id)));
        }

        /// <summary>
        /// 抓取商品区域购买限制
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchSkuAreaLimit(JDFetchSkuAreaLimitIn areaLimitIn)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchSkuAreaLimit(areaLimitIn)));
        }

        /// <summary>
        /// 抓取运费
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchFreight(JDFetchFreightIn freightIn)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchFreight(freightIn)));
        }

        /// <summary>
        /// 抓取商品可售信息（返回结果包含是否支持7天无理由退货）
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchSkuIsForSale(long id)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchSkuIsForSale(id)));
        }

        /// <summary>
        /// 抓取单个商品京东价格
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchJDPrice(long id)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchSkuJDPrice(id)));
        }

        /// <summary>
        /// 抓取多个商品京东价格
        /// </summary>
        /// <param name="skuIds"></param>
        /// <returns></returns>
        public ActionResult FetchJDPrices(IEnumerable<long> skuIds)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchSkuJDPrice(skuIds)));
        }

        /// <summary>
        /// 抓取京东全部分类（品类）列表
        /// 抓取前请手动清空分类表
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchJDCatetory()
        {
            _jdService.FetchJDCategory();
            return Content("OK");
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchBalance(int paymentComId)
        {
            var result = _jdService.FetchBalance(paymentComId);

            return Content(result.ToString());
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult SubmitOrder(JDSubmitOrderIn order)
        {
            var result = _jdService.SubmitOrder(order);

            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public ActionResult ConfirmOrder(int paymentCompanyId, string jdOrderId)
        {
            return Content(JsonConvert.SerializeObject(_jdService.ConfirmOrder(paymentCompanyId, jdOrderId)));
        }

        /// <summary>
        /// 取消未确认订单
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public ActionResult CancelOrder(int paymentCompanyId, string jdOrderId)
        {
            return Content(_jdService.CancelOrder(paymentCompanyId, jdOrderId).ToString());
        }

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public ActionResult DoPay(int paymentCompanyId, string jdOrderId)
        {
            return Content(JsonConvert.SerializeObject(_jdService.DoPay(paymentCompanyId, jdOrderId)));
        }

        /// <summary>
        /// 订单反查
        /// 返回值为京东的订单号
        /// </summary>
        /// <param name="nopOrderId">Nop订单编号</param>
        /// <returns></returns>
        public ActionResult SearchJDOrderByNopOrderId(int paymentCompanyId, string nopOrderId)
        {
            return Content(JsonConvert.SerializeObject(_jdService.SearchJDOrderByNopOrderId(paymentCompanyId, nopOrderId)));
        }

        public ActionResult FetchJDOrderDetail(int paymentCompanyId, string jdOrderId)
        {
            return Content(JsonConvert.SerializeObject(_jdService.FetchJDOrderDetail(paymentCompanyId, jdOrderId)));
        }

        /// <summary>
        /// 查询配送信息
        /// 返回值JSON类型
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        public ActionResult OrderTrack(int paymentCompanyId, string jdOrderId)
        {
            return Content(JsonConvert.SerializeObject(_jdService.OrderTrack(paymentCompanyId, jdOrderId)));
        }

        /// <summary>
        /// 抓取消息推送消息
        /// </summary>
        /// <returns></returns>
        public ActionResult FetchJDMessage(int? paymentCompanyId, string type)
        {
            JDMessageOut json = null;
            if (paymentCompanyId > 0)
            {
                json = _jdService.FetchJDMessage(paymentCompanyId.Value, type);
            }
            else
            {
                json = _jdService.FetchJDMessage(type);
            }

            return Content(JsonConvert.SerializeObject(json));
        }

        /// <summary>
        /// 删除京东消息
        /// </summary>
        /// <param name="paymentCompanyId"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public ActionResult DeleteJDMessage(int? paymentCompanyId, string msgId)
        {
            JDBoolOut json = null;
            if (paymentCompanyId > 0)
            {
                json = _jdService.DeleteJDMessage(paymentCompanyId.Value, msgId);
            }
            else
            {
                json = _jdService.DeleteJDMessage(msgId);
            }

            return Content(JsonConvert.SerializeObject(json));
        }

        public ActionResult FetchJDPriceChange()
        {
            var changes = _jdService.FetchJDPriceChange();

            return Content(JsonConvert.SerializeObject(changes));
        }

        public ActionResult FetchJDSkuStateChange()
        {
            var changes = _jdService.FetchJDSkuStateChange();

            return Content(JsonConvert.SerializeObject(changes));
        }

        public ActionResult FetchLogisticsState(int paymentCompanyId)
        {
            var changes = _jdService.FetchLogisticsState(paymentCompanyId);

            return Content(JsonConvert.SerializeObject(changes));
        }


        #region 售后

        public ActionResult AS_GetAvailableNum(int paymentCompanyId, string jdOrderId, long skuId)
        {
            return Content(
                JsonConvert.SerializeObject(_jdService.AS_GetAvailableNum(paymentCompanyId, jdOrderId, skuId)));
        }

        public ActionResult AS_GetAferSaleType(int paymentCompanyId, string jdOrderId, long skuId)
        {
            return Content(
                JsonConvert.SerializeObject(_jdService.AS_GetAferSaleType(paymentCompanyId, jdOrderId, skuId)));
        }

        public ActionResult AS_GetReturnType(int paymentCompanyId, string jdOrderId, long skuId)
        {
            return Content(
                JsonConvert.SerializeObject(_jdService.AS_GetReturnType(paymentCompanyId, jdOrderId, skuId)));
        }

        public ActionResult AS_CreateAfterSaleOrder(JDAfterSaleIn afterOrder)
        {
            return Content(JsonConvert.SerializeObject(_jdService.AS_CreateAfterSaleOrder(afterOrder)));
        }

        public ActionResult AS_GetAferSaleOrderListByJDOrderId(int paymentCompanyId, string jdOrderId)
        {
            return Content(
                JsonConvert.SerializeObject(
                    _jdService.AS_GetAferSaleOrderListByJDOrderId(paymentCompanyId, jdOrderId)));
        }

        public ActionResult AS_GetAferSaleOrderDetail(int paymentCompanyId, int afsServiceId,
            JDAfterSaleOrderDetailTypeEnum?[] detailTypeEnum)
        {
            return Content(
                JsonConvert.SerializeObject(
                    _jdService.AS_GetAferSaleOrderDetail(paymentCompanyId, afsServiceId, detailTypeEnum)));
        }

        public ActionResult AS_CancelAfterSaleOrder(int paymentCompanyId, string reason, params int[] afterSaleOrderIds)
        {
            return Content(
                JsonConvert.SerializeObject(
                    _jdService.AS_CancelAfterSaleOrder(paymentCompanyId, reason, afterSaleOrderIds)));
        }
        #endregion
    }
}