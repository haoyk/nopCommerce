using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDFetchSkuAreaLimitIn
    {
        public IEnumerable<int> skuIds { get; set; }

        public int provinceId { get; set; }

        public int cityId { get; set; }

        public int countyId { get; set; }

        public int townId { get; set; }
    }
}
