using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;
using Nop.Core.JD;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;
using Nop.Services.JD.Model;

namespace Nop.Services.JD
{
    public interface IJDService
    {
        string CallApi(string url, string inputParams);

        JDToken JDCommonToken { get; set; }

        /// <summary>
        /// 获取京东Token
        /// </summary>
        /// <param name="jdClientInfo">获取京东Token所需的配置信息</param>
        /// <summary>该方法会处理请求京东失败的情况，失败后会重试请求，失败达到一定次数后返回Null</summary>
        /// <returns></returns>
        JDToken GetJDToken(JDClientInfo jdClientInfo);

        /// <summary>
        /// 刷新京东Token
        /// </summary>
        /// <summary>该方法会处理请求京东失败的情况，失败后会重试请求，失败达到一定次数后返回Null</summary>
        /// <returns></returns>
        JDToken RefreshToken(string refreshToken, string clientId, string clientSecret);

        /// <summary>
        /// 获取商品详细信息
        /// </summary>
        /// <param name="skuId">商品编号</param>
        /// <returns></returns>
        JDSkuDetail FetchSkuDetail(long skuId);

        /// <summary>
        /// 抓取商品上下架状态
        /// </summary>
        /// <returns></returns>
        bool FetchSkuState(long skuId);

        /// <summary>
        /// 抓取商品图片
        /// </summary>
        /// <param name="skuId">商品编号</param>
        /// <returns></returns>
        IList<JDSkuImageOut> FetchSkuImage(long skuId);

        /// <summary>
        /// 抓取商品区域购买限制
        /// </summary>
        /// <returns></returns>
        Dictionary<long, bool> FetchSkuAreaLimit(JDFetchSkuAreaLimitIn areaLimitIn);

        /// <summary>
        /// 抓取运费
        /// </summary>
        /// <param name="freightIn"></param>
        /// <returns></returns>
        JDFetchFreightOut FetchFreight(JDFetchFreightIn freightIn);

        /// <summary>
        /// 抓取商品可售信息（返回结果包含是否支持7天无理由退货）
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        JDFetchIsForSaleOut FetchSkuIsForSale(long skuId);

        /// <summary>
        /// 抓取单个商品京东价格
        /// </summary>
        /// <returns></returns>
        JDFetchPriceOut FetchSkuJDPrice(long skuId);

        /// <summary>
        /// 抓取多个个商品京东价格
        /// </summary>
        /// <returns></returns>
        IList<JDFetchPriceOut> FetchSkuJDPrice(IEnumerable<long> skuIds);

        /// <summary>
        /// 批量获取最新京东商品价格
        /// </summary>
        void BatchUpdateSkuJDPrice();

        /// <summary>
        /// 批量获取京东库存
        /// </summary>
        void BatchUpdateSkuJDStock();

        /// <summary>
        /// 抓取京东全部分类（品类）列表
        /// 抓取前请手动清空分类表
        /// </summary>
        void FetchJDCategory();

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        decimal FetchBalance(int paymentCompanyId);

        void FetchAddr();

        /// <summary>
        /// 使用公用Client获取京东推送信息
        /// </summary>
        /// <param name="type">推送类型，支持多个组合，例如 1,2,3。为空时返回全部类型</param>
        /// <returns></returns>
        JDMessageOut FetchJDMessage(string type);

        /// <summary>
        /// 根据发并可公司获取京东推送信息
        /// </summary>
        /// <param name="type">推送类型，支持多个组合，例如 1,2,3。为空时返回全部类型</param>
        /// <param name="paymentCompanyId"></param>
        /// <returns></returns>
        JDMessageOut FetchJDMessage(int paymentCompanyId, string type);

        /// <summary>
        /// 使用公用Client删除京东消息
        /// </summary>
        /// <param name="msgId">京东消息编码</param>
        /// <returns></returns>
        JDBoolOut DeleteJDMessage(string msgId);

        /// <summary>
        /// 删除京东消息
        /// </summary>
        /// <param name="msgId">京东消息编码</param>
        /// <returns></returns>
        JDBoolOut DeleteJDMessage(int paymentCompanyId, string msgId);

        /// <summary>
        /// 抓取京东商品价格变化
        /// </summary>
        /// <returns></returns>
        JDPriceChangeOut FetchJDPriceChange();

        /// <summary>
        /// 抓取京东商品上下架状态
        /// </summary>
        /// <returns></returns>
        JDSkuStateChangeOut FetchJDSkuStateChange();

        /// <summary>
        /// 抓取京东订单物流状态
        /// </summary>
        /// <returns></returns>
        JDLogisticsStateOut FetchLogisticsState(int paymentCompanyId);

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        JDSubmitOrderOut SubmitOrder(JDSubmitOrderIn order);

        /// <summary>
        /// 确认订单
        /// 订单确认后京东会异步发起支付，不再需要调用发起支付接口了。
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        JDBoolOut ConfirmOrder(int paymentCompanyId, string jdOrderId);

        /// <summary>
        /// 取消未确认订单
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        JDBoolOut CancelOrder(int paymentCompanyId, string jdOrderId);

        /// <summary>
        /// 发起支付
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        JDBoolOut DoPay(int paymentCompanyId, string jdOrderId);

        /// <summary>
        /// 订单反查
        /// 返回值为京东的订单号
        /// </summary>
        /// <param name="nopOrderId">Nop订单编号</param>
        /// <returns></returns>
        JDOrderIdOut SearchJDOrderByNopOrderId(int paymentCompanyId, string nopOrderId);

        /// <summary>
        /// 抓取京东订单详情
        /// 此方法返回集合类型，当京东没有拆单的情况下只返回一个对象，京东拆单时返回多个对象。
        /// 此方法在支付完成后才可调用，否则无法返回东京拆单明细
        /// </summary>
        /// <returns></returns>
        JDOrderDetailOut FetchJDOrderDetail(int paymentCompanyId, string jdOrderId);

        /// <summary>
        /// 查询配送信息
        /// 返回值JSON类型
        /// </summary>
        /// <param name="jdOrderId">京东订单编号</param>
        /// <returns></returns>
        JDOrderTrackOut OrderTrack(int paymentCompanyId, string jdOrderId);

        /// <summary>
        /// 获取商品可提交售后数据
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        JDIntOut AS_GetAvailableNum(int paymentCompanyId, string jdOrderId, long skuId);

        /// <summary>
        /// 获取商品支持的服务类型
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        JDCodeNameListOut AS_GetAferSaleType(int paymentCompanyId, string jdOrderId, long skuId);

        /// <summary>
        /// 获取商品返回京东方式
        /// </summary>
        /// <param name="jdOrderId"></param>
        /// <param name="skuId"></param>
        /// <returns></returns>
        JDCodeNameListOut AS_GetReturnType(int paymentCompanyId, string jdOrderId, long skuId);

        /// <summary>
        /// 创建售后服务单
        /// 需要该配送单已经妥投。
        /// 需要先调用10.3接口校验订单中某商品是否可以提交售后服务
        /// 需要先调用10.4接口查询支持的服务类型
        /// 需要先调用10.5接口查询支持的商品返回京东方式
        /// </summary>
        /// <returns></returns>
        JDBoolOut AS_CreateAfterSaleOrder(JDAfterSaleIn afterOrder);

        /// <summary>
        /// 分页查询服务单概要信息
        /// 注意:如此订单反复提交过服务单,本方法将返回所有记录,请注意筛选
        /// </summary>
        /// <param name="jdOrderId">京东订单号</param>
        /// <param name="pageSize">每页大小，默认值100</param>
        /// <param name="pageIndex">页码，1代表第一页</param>
        /// <returns></returns>
        JDAfterSaleOrderSummaryOut AS_GetAferSaleOrderListByJDOrderId(int paymentCompanyId, string jdOrderId, int pageSize = 100, int pageIndex = 1);

        /// <summary>
        /// 查询服务单详细信息
        /// </summary>
        /// <param name="afsServiceId">服务单号</param>
        /// <param name="detailTypeEnum">京东服务状态详细类型</param>
        /// <returns></returns>
        JDAfterSaleOrderDetailOut AS_GetAferSaleOrderDetail(int paymentCompanyId, int afsServiceId,
            JDAfterSaleOrderDetailTypeEnum?[] detailTypeEnum);

        /// <summary>
        /// 取消售后服务单
        /// </summary>
        /// <param name="reason">取消原因</param>
        /// <param name="afterSaleOrderIds">待取消售后服务单号</param>
        /// <returns></returns>
        JDBoolOut AS_CancelAfterSaleOrder(int paymentCompanyId, string reason, params int[] afterSaleOrderIds);
    }
}
