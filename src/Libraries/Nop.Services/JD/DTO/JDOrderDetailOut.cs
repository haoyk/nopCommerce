using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.DTO
{
    public class JDOrderDetailOut: JDOutBase
    {
        public List<JDOrderDetailOut_Data> Data { get; set; }
    }

    public class JDOrderDetailOut_Data
    {
        public string JDOrderId { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public LogisticsStateEnum LogisticsState { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public OrderTypeEnum OrderType { get; set; }

        public decimal OrderPrice { get; set; }

        /// <summary>
        /// 订单裸价
        /// </summary>
        public decimal OrderNakedPrice { get; set; }

        /// <summary>
        /// 订单税额
        /// </summary>
        public decimal OrderTaxPrice { get; set; }

        public decimal Freight { get; set; }

        public string ParentOrder { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStateEnum OrderState { get; set; }

        /// <summary>
        /// 确认状态
        /// </summary>
        public SubmitStateEnum SubmitState { get; set; }

        /// <summary>
        /// 订单商品
        /// </summary>
        public List<JDOrder_Sku> SkuList { get; set; }
    }

    public class JDOrder_Sku
    {
        public long SkuId { get; set; }

        public int Num { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public string Name { get; set; }

        public decimal Tax { get; set; }

        public decimal TaxPrice { get; set; }

        public decimal NakedPrice { get; set; }

        /// <summary>
        /// 0普通、1附件、2赠品
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 主商品skuid,如果本身是主商品，则oid为0
        /// </summary>
        public int Oid { get; set; }

    }
}
