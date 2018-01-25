using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.Model
{
    public class JDSkuStateResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDSkuStateResult_Detail> result { get; set; }
    }

    public class JDSkuStateResult_Detail
    {
        public long sku { get; set; }

        public int state { get; set; }

        public bool State => state == 1;
    }
}
