using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.JD.DTO.Enum;

namespace Nop.Services.JD.DTO
{
    public class JDAfterSaleIn
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public int PaymentCompanyId { get; set; }

        [JsonProperty(PropertyName = "jdOrderId")]
        public string JdOrderId { get; set; }

        /// <summary>
        /// 客户预期,退货(10)、换货(20)、维修(30)
        /// </summary>
        [JsonProperty(PropertyName = "customerExpect")]
        public CustomerExpectEnum CustomerExpect { get; set; }

        /// <summary>
        /// 产品问题描述,最多1000字符
        /// </summary>
        [JsonProperty(PropertyName = "questionDesc")]
        public string QuestionDesc { get; set; }

        /// <summary>
        /// 是否需要检测报告
        /// </summary>
        [JsonProperty(PropertyName = "isNeedDetectionReport")]
        public bool IsNeedDetectionReport => false;

        /// <summary>
        /// 问题描述图片  String 最多2000字符
        /// 支持多张图片，用逗号分隔（英文逗号）
        /// </summary>
        [JsonProperty(PropertyName = "questionPic")]
        public string QuestionPic { get; set; }

        /// <summary>
        /// 是否有包装
        /// </summary>
        [JsonProperty(PropertyName = "isHasPackage")]
        public bool IsHasPackage { get; set; }

        /// <summary>
        /// 包装描述
        /// 0 无包装 
        /// 10 包装完整 
        /// 20 包装破损
        /// </summary>
        [JsonProperty(PropertyName = "packageDesc")]
        public JDPackageDescEnum PackageDesc { get; set; }

        /// <summary>
        /// 客户信息实体
        /// </summary>
        [JsonProperty(PropertyName = "asCustomerDto")]
        public JDAfterSaleIn_CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// 取件信息实体
        /// </summary>
        [JsonProperty(PropertyName = "asPickwareDto")]
        public JDAfterSaleIn_PickwareInfo PickwareInfo { get; set; }

        /// <summary>
        /// 返件信息实体,即商品如何返回客户手中
        /// </summary>
        [JsonProperty(PropertyName = "asReturnwareDto")]
        public JDAfterSaleIn_ReturnwareInfo ReurnwareInfo { get; set; }

        /// <summary>
        /// 申请单明细
        /// </summary>
        [JsonProperty(PropertyName = "asDetailDto")]
        public JDAfterSaleIn_Item SkuInfo { get; set; }

        public void Check()
        {
            (PaymentCompanyId == default(int)).TrueThrow("PaymentCompanyId不能为空");
            JdOrderId.NullOrEmptyCheck("JdOrderId");
            QuestionDesc.NullOrEmptyCheck("产品问题描述");
            (QuestionDesc.Length > 1000).TrueThrow("产品问题描述最多1000字符");
            (QuestionPic.Length > 2000).TrueThrow("问题描述图片最多2000字符");
            (CustomerInfo == null).TrueThrow("客户信息不能为空");
            (PickwareInfo == null).TrueThrow("取件信息不能为空");

            ((CustomerExpect == CustomerExpectEnum.换货 || CustomerExpect == CustomerExpectEnum.维修)
                && PickwareInfo == null).TrueThrow("返件信息不能为空");
        }
    }

    /// <summary>
    /// 客户信息
    /// </summary>
    public class JDAfterSaleIn_CustomerInfo
    {
        /// <summary>
        /// 联系人
        /// </summary>
        [JsonProperty(PropertyName = "customerContactName")]
        public string CustomerContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [JsonProperty(PropertyName = "customerTel")]
        public string CustomerTel { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [JsonProperty(PropertyName = "customerMobilePhone")]
        public string CustomerMobilePhone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty(PropertyName = "customerEmail")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [JsonProperty(PropertyName = "customerPostcode")]
        public string CustomerPostcode { get; set; }
    }

    /// <summary>
    /// 取件信息
    /// </summary>
    public class JDAfterSaleIn_PickwareInfo
    {
        /// <summary>
        /// 取件方式
        /// 4 上门取件
        /// 7 客户送货
        /// 40客户发货
        /// </summary>
        [JsonProperty(PropertyName = "pickwareType")]
        public JDPickwareTypeEnum PickwareType { get; set; }

        /// <summary>
        /// 取件省
        /// </summary>
        [JsonProperty(PropertyName = "pickwareProvince")]
        public int PickwareProvince { get; set; }

        /// <summary>
        /// 取件市
        /// </summary>
        [JsonProperty(PropertyName = "pickwareCity")]
        public int PickwareCity { get; set; }

        /// <summary>
        /// 取件区县
        /// </summary>
        [JsonProperty(PropertyName = "pickwareCounty")]
        public int PickwareCounty { get; set; }

        /// <summary>
        /// 取件乡镇
        /// </summary>
        [JsonProperty(PropertyName = "pickwareVillage")]
        public int PickwareVillage { get; set; }

        /// <summary>
        /// 取件街道地址。最多500字符, 必填
        /// </summary>
        [JsonProperty(PropertyName = "pickwareAddress")]
        public string PickwareAddress { get; set; }
    }

    /// <summary>
    /// 返件信息
    /// </summary>
    public class JDAfterSaleIn_ReturnwareInfo
    {
        /// <summary>
        ///返件方式。 自营配送(10),第三方配送(20); 换、修这两种情况必填（默认值）
        /// </summary>
        [JsonProperty(PropertyName = "returnwareType")]
        public ReturnwareTypeEnum ReturnwareType { get; set; }

        /// <summary>
        /// 返件省
        /// </summary>
        [JsonProperty(PropertyName = "returnwareProvince")]
        public int ReturnwareProvince { get; set; }

        /// <summary>
        /// 返件市
        /// </summary>
        [JsonProperty(PropertyName = "returnwareCity")]
        public int ReturnwareCity { get; set; }

        /// <summary>
        /// 返件县
        /// </summary>
        [JsonProperty(PropertyName = "returnwareCounty")]
        public int ReturnwareCounty { get; set; }

        /// <summary>
        /// 返件乡镇
        /// </summary>
        [JsonProperty(PropertyName = "returnwareVillage")]
        public int ReturnwareVillage { get; set; }

        /// <summary>
        /// 返件街道地址
        /// </summary>
        [JsonProperty(PropertyName = "returnwareAddress")]
        public string ReturnwareAddress { get; set; }
    }

    public class JDAfterSaleIn_Item
    {
        [JsonProperty(PropertyName = "skuId")]
        public long SkuId { get; set; }

        [JsonProperty(PropertyName = "skuNum")]
        public int SkuNum { get; set; }
    }

}
