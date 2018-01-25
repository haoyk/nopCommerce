using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    /// <summary>
    /// 京东上下架状态变更
    /// </summary>
    public class JDSkuStateChangeResult : JDResultBase
    {
        public List<JDSkuStateChangeResult_Detail> result { get; set; }
    }

    public class JDSkuStateChangeResult_Detail
    {

        public string id { get; set; }

        public DateTime time { get; set; }

        public string type { get; set; }

        public JDSkuStateChangeResult_Sku result { get; set; }
    }

    public class JDSkuStateChangeResult_Sku
    {

        public long skuId { get; set; }

        public int state { get; set; }
    }
}
