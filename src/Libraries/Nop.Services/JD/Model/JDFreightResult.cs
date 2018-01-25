using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Services.JD.DTO;

namespace Nop.Services.JD.Model
{
    public class JDFreightResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDFreightResult_Detail result { get; set; }
    }

    public class JDFreightResult_Detail
    {
        public decimal freight { get; set; }

        public decimal baseFreight { get; set; }

        public decimal remoteRegionFreight { get; set; }

        public string remoteSku { get; set; }

        public JDFetchFreightOut ToJDFetchFreightOut()
        {
            return new JDFetchFreightOut()
            {
                BaseFreight = this.baseFreight,
                Freight = this.freight,
                remoteRegionFreight = this.remoteRegionFreight,
                RemoteSku = this.remoteSku
            };
        }
    }
}
