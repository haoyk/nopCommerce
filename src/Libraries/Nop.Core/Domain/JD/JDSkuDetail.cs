using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Core.Domain.JD
{
    public class JDSkuDetail: BaseEntity
    {
        public long Sku { get; set; }

        public string SaleUnit { get; set; }

        public string Weight { get; set; }

        public string ProductArea { get; set; }

        public string WareQD { get; set; }

        public string ImagePath { get; set; }

        public string Param { get; set; }

        public int State { get; set; }

        public string Shouhou { get; set; }

        public string BrandName { get; set; }

        public string UPC { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public string Introduction { get; set; }

        /// <summary>
        /// eleGift
        /// </summary>
        public string EleGift { get; set; }

        [JsonIgnore]
        public string Json { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
