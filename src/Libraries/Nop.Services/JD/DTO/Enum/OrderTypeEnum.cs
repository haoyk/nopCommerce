using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 订单类型   1是父订单   2是子订单
    /// </summary>
    public enum OrderTypeEnum
    {
        父订单 = 1,
        子订单 = 2
    }
}
