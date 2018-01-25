using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.JD;

namespace Nop.Services.JD.Model
{
    public class JDSkuDetailResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDSkuDetailResult_Detail result { get; set; }

        public JDSkuDetail ToJDSkuDetail()
        {
            return new JDSkuDetail()
            {
                Sku = this.result.sku,
                SaleUnit = this.result.saleUnit,
                Weight = this.result.weight,
                ProductArea = this.result.productArea,
                WareQD = this.result.wareQD,
                ImagePath = this.result.imagePath,
                Param = this.result.param,
                State = this.result.state,
                Shouhou = this.result.shouhou,
                BrandName = this.result.brandName,
                UPC = this.result.upc,
                Category = this.result.category,
                Name = this.result.name,
                Introduction = this.result.introduction,
                EleGift = this.result.eleGift,
                CreateTime = DateTime.Now
            };
        }
    }

    public class JDSkuDetailResult_Detail
    {
        public string saleUnit { get; set; }

        public string weight { get; set; }

        public string productArea { get; set; }

        public string wareQD { get; set; }

        public string imagePath { get; set; }

        public string param { get; set; }

        public int state { get; set; }

        public long sku { get; set; }

        public string shouhou { get; set; }

        public string brandName { get; set; }

        public string upc { get; set; }

        public string category { get; set; }

        public string name { get; set; }

        public string introduction { get; set; }

        /// <summary>
        /// eleGift
        /// </summary>
        public string eleGift { get; set; }

        public string appintroduce { get; set; }
    }
}
