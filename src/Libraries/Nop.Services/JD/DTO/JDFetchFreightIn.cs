using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.DTO
{
    public class JDFetchFreightIn
    {
        public List<JDFetchFreightIn_Skus> Skus { get; set; }

        public int Province { get; set; }

        public int City { get; set; }

        public int County { get; set; }

        public int Town { get; set; }

        public int PaymentType => 4;
    }

    public class JDFetchFreightIn_Skus
    {
        [JsonProperty(PropertyName = "skuId")]
        public long SkuId { get; set; }

        [JsonProperty(PropertyName = "num")]
        public int Num { get; set; }
    }
}
