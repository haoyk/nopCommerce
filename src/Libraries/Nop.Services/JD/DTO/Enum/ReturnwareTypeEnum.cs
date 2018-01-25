using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 返件方式。 自营配送(10),第三方配送(20); 换、修这两种情况必填（默认值）
    /// </summary>
    public enum ReturnwareTypeEnum
    {
        自营配送 = 10,
        第三方配送 = 20
    }
}
