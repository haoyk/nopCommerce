using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Services.JD.DTO;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.Model
{
    public class JDASOSummaryResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDASOSummaryResult_Detail result { get; set; }

        public JDAfterSaleOrderSummaryOut_Detail ToOut()
        {
            if (result == null)
                return null;

            return new JDAfterSaleOrderSummaryOut_Detail()
            {
                PageIndex = result.pageIndex,
                PageNum = result.pageNum,
                PageSize = result.pageSize,
                TotalNum = result.totalNum,
                ServiceInfoList = result.serviceInfoList?.Select(p => new JDAfterSaleOrderSummaryOut_List()
                {
                    AFSApplyTime = p.afsApplyTime,
                    AFSServiceId = p.afsServiceId,
                    AFSServiceStep = (AfterSaleOrderStepEnum)p.afsServiceStep,
                    AFSServiceStepName = p.afsServiceStepName,
                    CanCancel = p.cancel == 1,
                    CustomerExpect = (CustomerExpectEnum)p.customerExpect,
                    CustomerExpectName = p.customerExpectName,
                    JDOrderId = p.orderId,
                    SkuId = p.wareId,
                    SkuName = p.wareName
                }).ToList()
            };
        }
    }

    public class JDASOSummaryResult_Detail
    {
        public int totalNum { get; set; }

        public int pageSize { get; set; }

        public int pageNum { get; set; }

        public int pageIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDASOSummaryResult_List> serviceInfoList { get; set; }

    }

    public class JDASOSummaryResult_List
    {
        public int afsServiceId { get; set; }

        public int customerExpect { get; set; }

        public string customerExpectName { get; set; }

        public DateTime afsApplyTime { get; set; }

        public string orderId { get; set; }

        public long wareId { get; set; }

        public string wareName { get; set; }

        public int afsServiceStep { get; set; }

        public string afsServiceStepName { get; set; }

        /// <summary>
        /// 0代表否，1代表是
        /// </summary>
        public int cancel { get; set; }
    }
}
