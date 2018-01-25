using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    /// <summary>
    /// 列表为空代表不允许操作
    /// 列表包含1代表取消
    /// 列表包含2代表允许填写或者修改客户发货信息
    /// </summary>
    public enum AfterSaleOrderAllowOperation
    {
        允许取消 = 1,
        填写或者修改客户发货信息 = 2
    }
}
