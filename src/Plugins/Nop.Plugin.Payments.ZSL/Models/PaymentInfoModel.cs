using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.ZSL.Models
{
    public class PaymentInfoModel
    {
        public PaymentInfoModel()
        {
            this.PaymentInfos = new List<PaymentInfoModel_Item>();
        }

        /// <summary>
        /// 发薪公司列表
        /// </summary>
        [DisplayName("发薪公司")]
        public List<PaymentInfoModel_Item> PaymentInfos { get; set; }
    }

    public class PaymentInfoModel_Item
    {
        [DisplayName("选中")]
        public bool Check { get; set; }

        [DisplayName("发薪公司编码")]
        public int PaymentCompanyId { get; set; }

        [DisplayName("发薪公司名称")]
        public string PaymentCompanyName { get; set; }

        [DisplayName("发薪公司余额")]
        public decimal Amount { get; set; }
    }
}
