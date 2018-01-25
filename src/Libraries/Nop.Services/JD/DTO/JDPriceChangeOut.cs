using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    /// <summary>
    /// 京东价格变化
    /// </summary>
    public class JDPriceChangeOut
    {
        /// <summary>
        /// Key为消息ID，Value为商品编码
        /// </summary>
        public Dictionary<string, long> Changes { get; set; }
    }
}
