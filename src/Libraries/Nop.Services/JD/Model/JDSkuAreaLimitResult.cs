using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDSkuAreaLimitResult : JDResultBase
    {
        /// <summary>
        /// 这里京东接口返回结果有问题，应该是个对象但京东返回了一个字符串
        /// </summary>
        public string result { get; set; }
    }

    public class SkuAreaLimitResult_Detail
    {
        public long skuId { get; set; }

        public bool isAreaRestrict { get; set; }
    }
}
