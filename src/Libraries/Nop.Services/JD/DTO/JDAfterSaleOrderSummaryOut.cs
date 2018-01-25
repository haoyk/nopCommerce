using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.DTO
{
    /// <summary>
    /// 分页服务单概要信息
    /// </summary>
    public class JDAfterSaleOrderSummaryOut : JDOutBase
    {
        public JDAfterSaleOrderSummaryOut_Detail Data { get; set; }
    }

    public class JDAfterSaleOrderSummaryOut_Detail
    {
        public int TotalNum { get; set; }

        public int PageSize { get; set; }

        public int PageNum { get; set; }

        public int PageIndex { get; set; }

        public List<JDAfterSaleOrderSummaryOut_List> ServiceInfoList { get; set; }
    }

    public class JDAfterSaleOrderSummaryOut_List
    {
        public int AFSServiceId { get; set; }

        /// <summary>
        /// 服务类型码,退货(10)、换货(20)、维修(30)
        /// </summary>
        public CustomerExpectEnum CustomerExpect { get; set; }

        /// <summary>
        /// 服务类型名称
        /// </summary>
        public string CustomerExpectName { get; set; }

        public DateTime AFSApplyTime { get; set; }

        public string JDOrderId { get; set; }

        public long SkuId { get; set; }

        public string SkuName { get; set; }

        /// <summary>
        /// 服务单环节
        /// </summary>
        public AfterSaleOrderStepEnum AFSServiceStep { get; set; }

        /// <summary>
        /// 服务单环节名称
        /// </summary>
        public string AFSServiceStepName { get; set; }

        /// <summary>
        /// 是否可取消
        /// </summary>
        public bool CanCancel { get; set; }
    }
}
