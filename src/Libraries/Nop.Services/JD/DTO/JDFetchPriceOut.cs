using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.Model;

namespace Nop.Services.JD.DTO
{
    public class JDFetchPriceOut
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public long SkuId { get; set; }

        /// <summary>
        /// 协议价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 京东原价
        /// </summary>
        public decimal JDPrice { get; set; }
    }


}
