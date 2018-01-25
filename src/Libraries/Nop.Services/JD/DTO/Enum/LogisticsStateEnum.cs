using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 物流状态
    /// 0 是新建  1是妥投   2是拒收
    /// </summary>
    public enum LogisticsStateEnum
    {
        新建 = 0,
        妥投 = 1,
        拒收 = 2
    }
}
