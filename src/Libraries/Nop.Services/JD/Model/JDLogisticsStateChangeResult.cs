using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDLogisticsStateChangeResult
    {
        public List<JDLogisticsStateChangeResult_Detail> result { get; set; }
    }

    public class JDLogisticsStateChangeResult_Detail
    {
        public string id { get; set; }

        public DateTime time { get; set; }

        public string type { get; set; }

        public JDLogisticsStateChangeResult_Order result { get; set; }
    }

    public class JDLogisticsStateChangeResult_Order
    {
        public string orderId { get; set; }

        public int state { get; set; }
    }
}
