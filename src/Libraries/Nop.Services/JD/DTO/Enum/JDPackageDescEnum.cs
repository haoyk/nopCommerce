using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 包装描述
    /// 0 无包装 
    /// 10 包装完整 
    /// 20 包装破损
    /// </summary>
    public enum JDPackageDescEnum
    {
        无包装 = 0,
        包装完整 = 10,
        包装破损 = 20
    }
}
