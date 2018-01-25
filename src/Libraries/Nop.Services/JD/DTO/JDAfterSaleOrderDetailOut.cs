using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.DTO
{
    /// <summary>
    /// 服务单详细信息
    /// </summary>
    public class JDAfterSaleOrderDetailOut : JDOutBase
    {
        public JDAfterSaleOrderDetailOut_Detail Data { get; set; }
    }

    public class JDAfterSaleOrderDetailOut_Detail
    {
        public int AFSServiceId { get; set; }

        /// <summary>
        /// 服务类型码,退货(10)、换货(20)、维修(20)
        /// </summary>
        public CustomerExpectEnum CustomerExpect { get; set; }

        public DateTime AFSApplyTime { get; set; }

        public string JDOrderId { get; set; }

        public bool IsHasInvoice { get; set; }

        public bool IsNeedDetectionReport { get; set; }

        public bool IsHasPackage { get; set; }

        public string QuestionPic { get; set; }

        /// <summary>
        /// 服务单环节
        /// 申请阶段(10),审核不通过(20),客服审核(21),商家审核(22),京东收货(31),商家收货(32), 
        /// 京东处理(33),商家处理(34), 用户确认(40),完成(50), 取消 (60);
        /// </summary>
        public AfterSaleOrderStepEnum AFSServiceStep { get; set; }

        /// <summary>
        /// 服务单环节名称
        /// </summary>
        public string AFSServiceStepName { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string ApproveNotes { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string QuestionDesc { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public AfterOrderApprovedResultEnum? ApprovedResult { get; set; }

        /// <summary>
        /// 审核结果描述
        /// </summary>
        public string ApprovedResultName { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public AfterSaleOrderProcessResultEnum? ProcessResult { get; set; }

        /// <summary>
        /// 处理结果描述
        /// </summary>
        public string ProcessResultName { get; set; }

        /// <summary>
        /// 列表为空代表不允许操作
        /// 列表包含1代表取消
        /// 列表包含2代表允许填写或者修改客户发货信息
        /// </summary>
        public List<AfterSaleOrderAllowOperation> AllowOperations { get; set; }

        /// <summary>
        /// 客户信息JSON
        /// </summary>
        public string CustomerInfo { get; set; }

        /// <summary>
        /// 售后地址信息JSON
        /// </summary>
        public string AddressInfo { get; set; }

        /// <summary>
        /// 客户发货信息JSON
        /// </summary>
        public string ExpressInfo { get; set; }

        /// <summary>
        /// 退款明细
        /// </summary>
        public List<JDAfterSaleOrderDetailOut_Finance> FinanceDetails { get; set; }

        /// <summary>
        /// 服务单追踪信息
        /// </summary>
        public string TrackInfo { get; set; }

        /// <summary>
        /// 服务单商品明细
        /// </summary>
        public string Skus { get; set; }
    }

    public class JDAfterSaleOrderDetailOut_Finance
    {
        public AfterSaleOrderRefundWayEnum RefundWay { get; set; }

        public string RefundWayName { get; set; }

        public AfterSaleOrderFinanceStatusEnum Status { get; set; }

        public string StatusName { get; set; }

        public decimal RefundPrice { get; set; }

        public string SkuName { get; set; }

        public long SkuId { get; set; }
    }
}
