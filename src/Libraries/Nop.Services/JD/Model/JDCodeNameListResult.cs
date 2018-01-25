using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.Model
{
    public class JDCodeNameListResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDCodeNameListResult_Detail> result { get; set; }
    }

    public class JDCodeNameListResult_Detail
    {
        public string code { get; set; }

        public string name { get; set; }
    }
}
