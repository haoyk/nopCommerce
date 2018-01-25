using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 处理结果
    /// 返修换新(23),退货(40), 换良(50),原返(60),病单(71),出检(72),维修(73),强制关单(80),线下换新(90)
    /// </summary>
    public enum AfterSaleOrderProcessResultEnum
    {
        返修换新 = 23,
        退货 = 40,
        换良 = 50,
        原返 = 60,
        病单 = 71,
        出检 = 72,
        维修 = 73,
        强制关单 = 80,
        线下换新 = 90
    }
}
