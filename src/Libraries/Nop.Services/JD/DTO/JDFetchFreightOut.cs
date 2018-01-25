using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDFetchFreightOut
    {
        /// <summary>
        /// 总运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 基础运费
        /// </summary>
        public decimal BaseFreight { get; set; }

        /// <summary>
        /// 偏远运费
        /// </summary>
        public decimal remoteRegionFreight { get; set; }

        /// <summary>
        /// 需收取偏远运费的sku
        /// </summary>
        public string RemoteSku { get; set; }
    }
}
