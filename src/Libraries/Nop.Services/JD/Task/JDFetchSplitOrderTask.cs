using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;
using Nop.Services.Logging;

namespace Nop.Services.JD.Task
{
    /// <summary>
    /// 异步获取京东拆单信息
    /// 由于京东拆单动作发生在付款（付款操作在东京是异步的）之后，
    /// 所以我们不能实时获取到拆单详细，只能后续抓取。
    /// PS:京东下单后会实时返回父订单号，该订单号已存储在了订单抬头表中
    /// </summary>
    public class JDFetchSplitOrderTask : IJDTask
    {
        #region privateService
        private ILogger _log => EngineContext.Current.Resolve<ILogger>();

        private IJDService _jdService = EngineContext.Current.Resolve<IJDService>();

        private IRepository<Order> _orderRep = EngineContext.Current.Resolve<IRepository<Order>>();

        private IRepository<OrderItem> _orderItemRep = EngineContext.Current.Resolve<IRepository<OrderItem>>();

        #endregion privateService

        public string TaskName => "异步获取京东拆单信息";

        public int Minutes => 10;

        public bool StopOnError => false;

        public void Execute()
        {
            _log.Information("计划任务-异步获取京东拆单信息开始");

            Process();

            _log.Information("计划任务-异步获取京东拆单信息结束");
        }

        private void Process()
        {
            var orders = GetNotFetchSplitOrer();

            orders.ForEach(order =>
            {
                try
                {
                    var splitInfo = FetchJDSplitInfo(order.PaymentCompanyId.Value, order.JdOrderId);

                    order.OrderItems.Where(item => item.JdOrderId.IsNullOrEmpty()).ToList().ForEach(item =>
                    {
                        //Single保证有且只有一个
                        var jdOrderDetailOutDatas = splitInfo.Single(split =>
                            {
                                return item.Product.JDSkuId != null &&
                                split.SkuList.Select(p => p.SkuId).Contains(item.Product.JDSkuId.Value);
                            });

                        if (jdOrderDetailOutDatas != null)
                        {
                            item.JdOrderId = jdOrderDetailOutDatas.JDOrderId;
                            _orderItemRep.Update(item);
                        }
                    });
                }
                catch (Exception e)
                {
                    _log.Error("异步获取京东拆单信息异常", e);
                }
            });
        }

        /// <summary>
        /// 获取待抓取拆单详细的订单
        /// </summary>
        /// <returns></returns>
        private List<Order> GetNotFetchSplitOrer()
        {
            //取创建后5分钟的订单，防止被就近轮询立即抓取到
            DateTime time = DateTime.UtcNow.AddMinutes(5);

            return _orderRep.Table.Where(p =>
                p.PaymentCompanyId != null && p.JdOrderId != null && p.Deleted == false &&
                p.OrderStatusId != (int)OrderStatus.Cancelled &&
                p.CreatedOnUtc > time &&
                p.OrderItems.Any(item => item.JdOrderId == null || item.JdOrderId == string.Empty)).ToList();
        }

        private List<JDOrderDetailOut_Data> FetchJDSplitInfo(int paymentCompanyId, string jdOrderId)
        {
            var jdResult = _jdService.FetchJDOrderDetail(paymentCompanyId, jdOrderId);
            if (jdResult.Success == false)
                throw new NopException(jdResult.ResultMessage);

            return jdResult.Data.Where(p => p.SubmitState == SubmitStateEnum.确认下单订单).ToList();
        }
    }
}
