using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.Model
{
    /// <summary>
    /// 京东订单拆单返回结果
    /// </summary>
    public class JDOrderSplitResult : JDResultBase
    {
        public JDOrderSplitResult_Result result { get; set; }

        public JDOrderDetailOut ToJDOrderDetailOut()
        {
            var list = new List<JDOrderDetailOut_Data>();
            if (this.result.pOrder != null && this.result.cOrder != null)
            {
                var pOrder = this.result.cOrder.FirstOrDefault();
                list.Add(new JDOrderDetailOut_Data()
                {
                    Freight = pOrder.freight,
                    JDOrderId = pOrder.jdOrderId,
                    LogisticsState = (LogisticsStateEnum)pOrder.state,
                    OrderPrice = pOrder.orderPrice,
                    OrderNakedPrice = pOrder.orderNakedPrice,
                    OrderTaxPrice = pOrder.orderTaxPrice,
                    OrderState = (OrderStateEnum)pOrder.orderState,
                    OrderType = (OrderTypeEnum)pOrder.type,
                    ParentOrder = pOrder.pOrder,
                    SubmitState = (SubmitStateEnum)pOrder.submitState,
                    SkuList = new List<JDOrder_Sku>(pOrder.sku.Select(p => p.ToJDOrder_Sku()))
                });

                var cOrder = this.result.cOrder.Where(p => !p.jdOrderId.Equals(pOrder.jdOrderId)).ToList();
                if (cOrder != null && cOrder.Any())
                {
                    list.AddRange(
                        cOrder.Select(c => new JDOrderDetailOut_Data()
                        {
                            Freight = c.freight,
                            JDOrderId = c.jdOrderId,
                            LogisticsState = (LogisticsStateEnum)c.state,
                            OrderPrice = c.orderPrice,
                            OrderNakedPrice = c.orderNakedPrice,
                            OrderTaxPrice = c.orderTaxPrice,
                            OrderState = (OrderStateEnum)c.orderState,
                            OrderType = (OrderTypeEnum)c.type,
                            ParentOrder = c.pOrder,
                            SubmitState = (SubmitStateEnum)c.submitState,
                            SkuList = new List<JDOrder_Sku>(c.sku.Select(p => p.ToJDOrder_Sku()))
                        })
                    );
                }
            }
            else
            {
                list = null;
            }

            return new JDOrderDetailOut()
            {
                Success = this.success,
                ResultCode = this.resultCode,
                ResultMessage = this.resultMessage,
                Data = list
            };
        }
    }

    public class JDOrderSplitResult_Result
    {
        /// <summary>
        /// 订单类型   1是父订单   2是子订单
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 订单状态  0为取消订单  1为有效
        /// </summary>
        public int orderState { get; set; }

        /// <summary>
        /// 0为未确认下单订单   1为确认下单订单
        /// </summary>
        public int submitState { get; set; }

        /// <summary>
        /// 父订单
        /// </summary>
        public JDOrderSplitResult_POrder pOrder { get; set; }

        /// <summary>
        /// 子订单
        /// </summary>
        public List<JDOrderSplitResult_COrder> cOrder { get; set; }
    }

    public class JDOrderSplitResult_POrder
    {
        public string jdOrderId { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal freight { get; set; }

        /// <summary>
        /// 订单价格
        /// </summary>
        public decimal orderPrice { get; set; }

        /// <summary>
        /// 订单裸价
        /// </summary>
        public decimal orderNakedPrice { get; set; }

        /// <summary>
        /// 订单税额
        /// </summary>
        public decimal orderTaxPrice { get; set; }

        public List<JDSubmitOrderResult_Sku> sku { get; set; }

    }

    public class JDOrderSplitResult_COrder
    {
        public string pOrder { get; set; }

        /// <summary>
        /// 订单状态  0为取消订单  1为有效
        /// </summary>
        public int orderState { get; set; }

        public string jdOrderId { get; set; }

        /// <summary>
        /// 物流状态 0 是新建  1是妥投   2是拒收
        /// </summary>
        public int state { get; set; }

        public decimal freight { get; set; }

        /// <summary>
        /// 0为未确认下单订单   1为确认下单订单
        /// </summary>
        public int submitState { get; set; }

        /// <summary>
        /// 订单价格
        /// </summary>
        public decimal orderPrice { get; set; }

        /// <summary>
        /// 订单裸价
        /// </summary>
        public decimal orderNakedPrice { get; set; }

        /// <summary>
        /// 订单类型   1是父订单   2是子订单
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 订单税额
        /// </summary>
        public decimal orderTaxPrice { get; set; }

        public List<JDSubmitOrderResult_Sku> sku { get; set; }
    }
}
