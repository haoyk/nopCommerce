using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 服务单环节
    /// 申请阶段(10),审核不通过(20),客服审核(21),商家审核(22),京东收货(31),商家收货(32), 
    /// 京东处理(33),商家处理(34), 用户确认(40),完成(50), 取消 (60);
    /// </summary>
    public enum AfterSaleOrderStepEnum
    {
        申请阶段 = 10,
        审核不通过 = 20,
        客服审核 = 21,
        商家审核 = 22,
        京东收货 = 31,
        商家收货 = 32,
        京东处理 = 33,
        商家处理 = 34,
        用户确认 = 40,
        完成 = 50,
        取消 = 60
    }
}
