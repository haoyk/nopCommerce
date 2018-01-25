using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO.Enum
{
    public enum JDMessageType
    {
        价格变更 = 2,
        商品上下架变更 = 4,
        订单已妥投 = 5,
        添加删除商品池内商品 = 6,
        支付失败 = 14,
        商品介绍及规格参数变更 = 16,
        京东地址变更消息推送 = 50
    }
}
