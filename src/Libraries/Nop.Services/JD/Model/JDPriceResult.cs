using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.Model
{
    public class JDPriceResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDPriceResult_Detail> result { get; set; }
    }

    public class JDPriceResult_Detail
    {
        public long skuId { get; set; }

        public decimal price { get; set; }

        public decimal jdPrice { get; set; }
    }
}
