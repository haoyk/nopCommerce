using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.JD;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;

namespace Nop.Services.JD.Task
{
    /// <summary>
    /// 自动设置京东已交货状态
    /// </summary>
    public class JDAutoDeliveredTask : IJDTask
    {
        private IOrderProcessingService _orderProcessingService => EngineContext.Current.Resolve<IOrderProcessingService>();
        private ILogger _log => EngineContext.Current.Resolve<ILogger>();

        private IJDService _jdService = EngineContext.Current.Resolve<IJDService>();

        private IShipmentService _shipmentService = EngineContext.Current.Resolve<IShipmentService>();

        private IRepository<Order> _orderRep = EngineContext.Current.Resolve<IRepository<Order>>();

        public string TaskName => "自动设置京东收货状态";

        public int Minutes => 180;

        public bool StopOnError => false;

        public void Execute()
        {
            _log.Information("计划任务-自动设置京东已交货状态开始");

            Process();

            _log.Information("计划任务-自动设置京东已交货状态结束");
        }

        #region MyRegion

        private void Process()
        {
            try
            {
                //获取待处理订单
                var orders = GetJDNotDeliveredOrders();

                //以发薪公司为维度获取京东物流状态
                orders.GroupBy(o => o.PaymentCompanyId).ToList().ForEach(payComId =>
                {
                    //某发薪公司下所有京东推送物流消息
                    var jdStates = _jdService.FetchLogisticsState(payComId.Key.Value);

                    payComId.ToList().ForEach(order =>
                    {
                        //NOP订单中所包含的所有京东订单ID
                        var oItemJDOrderIds = order.OrderItems.Select(oItem => oItem.JdOrderId);

                        //京东已妥投的订单
                        var jdDelivered = jdStates.States.Where(s =>
                                     s.State == LogisticsStateEnum.妥投 && oItemJDOrderIds.Contains(s.JDOrderId));

                        //NOP订单中全部子项都已妥投
                        if (oItemJDOrderIds.Count() == jdDelivered.Count())
                        {
                            MarkAsDeliveredAndDelJDMsg(order.Id, jdDelivered.Select(p => p.MessageId).ToArray());
                        }
                    });
                });
            }
            catch (Exception e)
            {
                _log.Error("自动设置京东已交货状态异常", e);
            }
        }

        private List<Order> GetJDNotDeliveredOrders()
        {
            return _orderRep.TableNoTracking.Where(p =>
                p.PaymentCompanyId != null && p.JdOrderId != null && p.Deleted == false && 
                p.OrderStatusId == (int)OrderStatus.Processing && p.PaymentStatusId == (int)PaymentStatus.Paid && 
                p.ShippingStatusId == (int)ShippingStatus.Shipped).ToList();
        }

        private void MarkAsDeliveredAndDelJDMsg(int nopOrderId, string[] jdMessageIds)
        {
            var shipment = _shipmentService.GetShipmentById(nopOrderId);
            if (shipment != null)
            {
                try
                {
                    //设置为已交货状态
                    _orderProcessingService.Deliver(shipment, true);

                    //删除京东推送消息
                    jdMessageIds.ToList().ForEach(msgId =>
                    {
                        _jdService.DeleteJDMessage(msgId);
                    });
                }
                catch (Exception e)
                {
                    _log.Error("标记已交货状态并删除京东消息异常", e);
                }
            }
        }

        #endregion

    }
}
