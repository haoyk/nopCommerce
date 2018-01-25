using Nop.Services.JD.DTO.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDSkuStateChangeOut
    {
        public List<JDSkuStateChange_Detail> Changes { get; set; }
    }

    public class JDSkuStateChange_Detail
    {
        public string MsgId { get; set; }

        public long SkuId { get; set; }

        public JDSkuState State { get; set; }

        public DateTime Time { get; set; }
    }
}
