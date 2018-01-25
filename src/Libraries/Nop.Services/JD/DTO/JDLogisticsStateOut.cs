using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.DTO
{
    public class JDLogisticsStateOut
    {
        /// <summary>
        /// 物流状态
        /// </summary>
        public List<JDLogisticsStateOut_Detail> States { get; set; }
    }

    public class JDLogisticsStateOut_Detail
    {
        public string MessageId { get; set; }

        /// <summary>
        /// 京东订单编码
        /// </summary>
        public string JDOrderId { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public LogisticsStateEnum State { get; set; }
    }
}
