using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDFetchIsForSaleOut
    {
        public long SkuId { get; set; }

        public string SkuName { get; set; }

        /// <summary>
        /// 是否可售
        /// </summary>
        public bool SaleState { get; set; }

        /// <summary>
        /// 是否可开增票
        /// </summary>
        public bool IsCanVAT { get; set; }

        /// <summary>
        /// 是否支持7天无理由退货
        /// </summary>
        public bool Is7ToReturn { get; set; }
    }
}
