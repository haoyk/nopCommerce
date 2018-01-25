using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDSubmitOrderOut
    {
        public bool Success { get; set; }

        public string ResultMessage { get; set; }

        public string ResultCode { get; set; }

        /// <summary>
        /// 京东主订单编号
        /// </summary>
        public string JDOrderId { get; set; }

        /// <summary>
        /// 总运费; 这个是订单总运费 = 基础运费 + 总的超重偏远附加运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 商品总价格
        /// </summary>
        public decimal OrderPrice { get; set; }

        /// <summary>
        /// 京东返回信息
        /// </summary>
        public string Json { get; set; }
    }
}
