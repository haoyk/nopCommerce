using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Services.JD.DTO;

namespace Nop.Services.JD.Model
{
    public class JDSubmitOrderResult : JDResultBase
    {
        public JDSubmitOrderResult_Detail result { get; set; }

        public JDSubmitOrderOut ToJDSubmitOrderOut()
        {
            //订单商品详细我们不关心，所以不返回
            return new JDSubmitOrderOut()
            {
                Success = this.success,
                ResultCode = this.resultCode,
                ResultMessage = this.resultMessage,
                JDOrderId = this.result.jdOrderId,
                Freight = this.result.freight,
                OrderPrice = this.result.orderPrice
            };
        }
    }

    public class JDSubmitOrderResult_Detail
    {
        /// <summary>
        /// 京东主订单编号
        /// </summary>
        public string jdOrderId { get; set; }

        /// <summary>
        /// 总运费; 这个是订单总运费 = 基础运费 + 总的超重偏远附加运费
        /// </summary>
        public decimal freight { get; set; }

        /// <summary>
        /// 商品总价格
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDSubmitOrderIn_Sku> sku { get; set; }
    }

    public class JDSubmitOrderResult_Sku
    {
        public long skuId { get; set; }

        public int num { get; set; }

        public string category { get; set; }

        public decimal price { get; set; }

        public string name { get; set; }

        public decimal tax { get; set; }

        public decimal taxPrice { get; set; }

        public decimal nakedPrice { get; set; }

        /// <summary>
        /// 0普通、1附件、2赠品
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 主商品skuid,如果本身是主商品，则oid为0
        /// </summary>
        public int oid { get; set; }

        public JDOrder_Sku ToJDOrder_Sku()
        {
            return new JDOrder_Sku()
            {
                Category = this.category,
                NakedPrice = this.nakedPrice,
                Name = this.name,
                Num = this.num,
                Oid = this.oid,
                Price = this.price,
                SkuId = this.skuId,
                Tax = this.tax,
                TaxPrice = this.taxPrice,
                Type = this.type
            };
        }
    }
}
