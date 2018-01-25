using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 京东服务状态详细类型
    /// 不设置数据表示只获取服务单主信息、商品明细以及客户信息；
    /// 1、代表增加获取售后地址信息 
    /// 2、代表增加获取客户发货信息 
    /// 3、代表增加获取退款明细 
    /// 4、增加获取跟踪信息 
    /// 5、获取允许的操作信息 
    /// </summary>
    public enum JDAfterSaleOrderDetailTypeEnum
    {
        全部 = 999,
        增加获取售后地址信息 = 1,
        增加获取客户发货信息 = 2,
        增加获取退款明细 = 3,
        增加获取跟踪信息 = 4,
        获取允许的操作信息 = 5
    }
}
