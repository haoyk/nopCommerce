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
    public class JDASODetailResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDASODetailResult_Detail result { get; set; }

        public JDAfterSaleOrderDetailOut_Detail ToOut()
        {
            if (result == null)
                return null;

            var returnVal = new JDAfterSaleOrderDetailOut_Detail();

            returnVal.AFSApplyTime = result.afsApplyTime;
            returnVal.AFSServiceId = result.afsServiceId;
            returnVal.AFSServiceStep = (AfterSaleOrderStepEnum)result.afsServiceStep;
            returnVal.AFSServiceStepName = result.afsServiceStepName;
            returnVal.AllowOperations = result.allowOperations;
            returnVal.ApprovedResult = (AfterOrderApprovedResultEnum?)result.approvedResult;
            returnVal.ApprovedResultName = result.approvedResultName;
            returnVal.ApproveNotes = result.approveNotes;
            returnVal.CustomerExpect = (CustomerExpectEnum)result.customerExpect;
            returnVal.FinanceDetails = result.serviceFinanceDetailInfoDTOs?.Select(p => new JDAfterSaleOrderDetailOut_Finance()
            {
                RefundPrice = p.refundPrice,
                RefundWay = (AfterSaleOrderRefundWayEnum)p.refundWay,
                RefundWayName = p.refundWayName,
                SkuId = p.wareId,
                SkuName = p.wareName,
                Status = (AfterSaleOrderFinanceStatusEnum)p.status,
                StatusName = p.statusName
            }).ToList();
            returnVal.IsHasInvoice = result.isHasInvoice == 1;
            returnVal.IsHasPackage = result.isHasPackage == 1;
            returnVal.IsNeedDetectionReport = result.isNeedDetectionReport == 1;
            returnVal.JDOrderId = result.orderId;
            returnVal.ProcessResult = (AfterSaleOrderProcessResultEnum?)result.processResult;
            returnVal.ProcessResultName = result.processResultName;
            returnVal.QuestionDesc = result.questionDesc;
            returnVal.QuestionPic = result.questionPic;
            returnVal.AddressInfo = JsonConvert.SerializeObject(result.serviceAftersalesAddressInfoDTO);
            returnVal.CustomerInfo = JsonConvert.SerializeObject(result.serviceCustomerInfoDTO);
            returnVal.ExpressInfo = JsonConvert.SerializeObject(result.serviceExpressInfoDTO);
            returnVal.Skus = JsonConvert.SerializeObject(result.serviceDetailInfoDTOs);
            returnVal.TrackInfo = JsonConvert.SerializeObject(result.serviceTrackInfoDTOs);

            return returnVal;
        }
    }

    public class JDASODetailResult_Detail
    {
        public int afsServiceId { get; set; }

        public int customerExpect { get; set; }

        public DateTime afsApplyTime { get; set; }

        public string orderId { get; set; }

        public int isHasInvoice { get; set; }

        public int isNeedDetectionReport { get; set; }

        public int isHasPackage { get; set; }

        public string questionPic { get; set; }

        public int afsServiceStep { get; set; }

        public string afsServiceStepName { get; set; }

        public string approveNotes { get; set; }

        public string questionDesc { get; set; }

        public int? approvedResult { get; set; }

        public string approvedResultName { get; set; }

        public int? processResult { get; set; }

        public string processResultName { get; set; }

        /// <summary>
        /// 列表为空代表不允许操作
        /// 列表包含1代表取消
        /// 列表包含2代表允许填写或者修改客户发货信息
        /// </summary>
        public List<AfterSaleOrderAllowOperation> allowOperations { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDASODetailResult_Customer serviceCustomerInfoDTO { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDASODetailResult_Addr serviceAftersalesAddressInfoDTO { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDASODetailResult_Express serviceExpressInfoDTO { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDASODetailResult_Finance> serviceFinanceDetailInfoDTOs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDASODetailResult_Track> serviceTrackInfoDTOs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDASODetailResult_Sku> serviceDetailInfoDTOs { get; set; }
    }

    public class JDASODetailResult_Customer
    {
        public string customerPin { get; set; }
        public string customerName { get; set; }
        public string customerContactName { get; set; }
        public string customerTel { get; set; }
        public string customerMobilePhone { get; set; }
        public string customerEmail { get; set; }
        public string customerPostcode { get; set; }

    }

    public class JDASODetailResult_Addr
    {
        /// <summary>
        /// 售后地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 售后电话
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 售后联系人
        /// </summary>
        public string linkMan { get; set; }

        public string postCode { get; set; }

    }

    public class JDASODetailResult_Express
    {
        /// <summary>
        /// 服务单号
        /// </summary>
        public int afsServiceId { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public string freightMoney { get; set; }

        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string expressCompany { get; set; }

        /// <summary>
        /// 客户发货日期
        /// </summary>
        public DateTime deliverDate { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string expressCode { get; set; }

    }

    public class JDASODetailResult_Finance
    {
        public int refundWay { get; set; }

        public string refundWayName { get; set; }

        public int status { get; set; }

        public string statusName { get; set; }

        public decimal refundPrice { get; set; }

        public string wareName { get; set; }

        public long wareId { get; set; }
    }

    public class JDASODetailResult_Track
    {
        public int afsServiceId { get; set; }
        public string title { get; set; }
        public string context { get; set; }
        public DateTime createDate { get; set; }

        /// <summary>
        /// 操作人昵称
        /// </summary>
        public string createName { get; set; }

        /// <summary>
        /// 操作人账号
        /// </summary>
        public string createPin { get; set; }

    }

    public class JDASODetailResult_Sku
    {
        public long wareId { get; set; }

        public string wareName { get; set; }

        /// <summary>
        /// 商品品牌
        /// </summary>
        public string wareBrand { get; set; }
        /// <summary>
        /// 明细类型
        /// 主商品(10), 赠品(20), 附件(30)，拍拍取主商品就可以
        /// </summary>
        public int afsDetailType { get; set; }

        /// <summary>
        /// 附件描述
        /// </summary>
        public string wareDescribe { get; set; }

    }
}
