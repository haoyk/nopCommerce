using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 0为未确认下单订单   1为确认下单订单
    /// </summary>
    public enum SubmitStateEnum
    {
        未确认下单订单 = 0,
        确认下单订单 = 1
    }
}
