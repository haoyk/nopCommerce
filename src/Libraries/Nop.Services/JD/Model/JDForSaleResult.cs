using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Services.JD.DTO;

namespace Nop.Services.JD.Model
{
    public class JDForSaleResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDForSaleResult_Detail> result { get; set; }
    }

    public class JDForSaleResult_Detail
    {
        public long skuId { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 是否可售
        /// </summary>
        public int saleState { get; set; }

        /// <summary>
        /// 是否可开增票
        /// </summary>
        public int isCanVAT { get; set; }

        /// <summary>
        /// 是否支持7天无理由退货
        /// </summary>
        public int is7ToReturn { get; set; }

        public JDFetchIsForSaleOut ToJDFetchIsForSaleOut()
        {
            return new JDFetchIsForSaleOut()
            {
                Is7ToReturn = this.is7ToReturn == 1,
                IsCanVAT = this.isCanVAT == 1,
                SaleState = this.saleState == 1,
                SkuId = this.skuId,
                SkuName = this.name
            };
        }
    }
}
