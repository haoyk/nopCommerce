using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 取件方式
    /// 4 上门取件
    /// 7 客户送货
    /// 40客户发货
    /// </summary>
    public enum JDPickwareTypeEnum
    {
        上门取件 = 4,
        客户送货 = 7,
        客户发货 = 40
    }
}
