using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDPriceChangeResult : JDResultBase
    {
        public List<JDPriceChangeResult_Detail> result { get; set; }
    }

    public class JDPriceChangeResult_Detail
    {
        public string id { get; set; }

        public DateTime time { get; set; }

        public string type { get; set; }

        public JDPriceChangeResult_Sku result { get; set; }
    }

    public class JDPriceChangeResult_Sku
    {
        public long skuId { get; set; }

        public string price { get; set; }

        public string jdPrice { get; set; }
    }
}
