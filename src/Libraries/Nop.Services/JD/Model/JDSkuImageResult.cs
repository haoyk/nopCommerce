using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.Model
{
    public class JDSkuImageResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<long, List<JDSkuImageResult_Detail>> result { get; set; }
    }

    public class JDSkuImageResult_Detail
    {
        public string path { get; set; }

        public int isPrimary { get; set; }

        public string orderSort { get; set; }
    }
}
