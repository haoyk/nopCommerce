using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDOrderTrackResult : JDResultBase
    {
        public string jdOrderId { get; set; }

        public object result { get; set; }
    }
}
