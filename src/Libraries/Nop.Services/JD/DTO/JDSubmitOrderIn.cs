using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.JD;
using Nop.Core.Infrastructure;

namespace Nop.Services.JD.DTO
{
    public class JDSubmitOrderIn
    {
        /// <summary>
        /// 发薪公司编码
        /// </summary>
        public int PaymentCompanyId { get; set; }

        /// <summary>
        /// 第三方订单编号
        /// </summary>
        public string NopOrderId { get; set; }

        /// <summary>
        /// 订单商品
        /// </summary>
        public List<JDSubmitOrderIn_Sku> Skus { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceiverName { get; set; }

        public int JDProvince { get; set; }

        public int JDCity { get; set; }

        public int JDCounty { get; set; }

        public int JDTown { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收货人手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 收货人邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 备注（少于100字）
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 开票方式(1为随货开票，0为订单预借，2为集中开票 )
        /// </summary>
        public int InvoiceState => 2;

        /// <summary>
        /// 1普通发票2增值税发票
        /// </summary>
        public int InvoiceType => 2;

        /// <summary>
        /// 发票类型：4个人，5单位
        /// </summary>
        public int SelectedInvoiceTitle => 5;

        private string _CompanyName;
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string CompanyName
        {
            get
            {
                if (_CompanyName == null)
                {
                    var _jdClient_PayComReq = EngineContext.Current.Resolve<IRepository<JDClientInfos_PaymentCompany>>();

                    var invoiceOjb = _jdClient_PayComReq.TableNoTracking.FirstOrDefault(p => p.PaymentComId == this.PaymentCompanyId);

                    _CompanyName = invoiceOjb?.CompanyInvoiceTitle;
                    _CompanyName.IsNullOrEmpty().TrueThrow("无法获取发薪公司抬头");
                }
                return _CompanyName;
            }
        }

        /// <summary>
        /// 1:明细，3：电脑配件，19:耗材，22：办公用品
        /// 备注:若增值发票则只能选1 明细
        /// </summary>
        public int InvoiceContent => 1;

        /// <summary>
        /// 支付方式 (1：货到付款，2：邮局付款，4：在线支付，5：公司转账，6：银行转账，7：网银钱包，101：金采支付)
        /// </summary>
        public int PaymentType => 4;

        /// <summary>
        /// 使用余额paymentType=4时，此值固定是1
        /// 其他支付方式0
        /// </summary>
        public int IsUseBalance => 1;

        /// <summary>
        /// 预占库存
        /// </summary>
        public int submitState => 0;

        /// <summary>
        /// 下单价格模式
        /// 1:必需验证客户端订单价格快照，如果快照与京东价格不一致返回下单失败，需要更新商品价格后，重新下单;
        /// </summary>
        public int DoOrderPriceMode => 1;

        /// <summary>
        /// 价格快照
        /// </summary>
        public List<JDSubmitOrderIn_PriceSnap> OrderPriceSnap { get; set; }

        public void Check()
        {
            (PaymentCompanyId <= 0).TrueThrow("下单输入参数发薪公司不能为空");
            (Remark.Length > 100).TrueThrow("备注不能大于100字");
            (OrderPriceSnap == null || !OrderPriceSnap.Any()).TrueThrow("价格快照不能为空");
            (Skus == null || !Skus.Any()).TrueThrow("订单商品不能为空");
            Skus.Any(p => p.Num <= 0 || p.JDSkuId <= 0).TrueThrow("订单商品或数量异常");
            Address.IsNullOrEmpty().TrueThrow("详细地址不能为空");
            Mobile.IsNullOrEmpty().TrueThrow("收货人手机号不能为空");
            Email.IsNullOrEmpty().TrueThrow("收货人EMail不能为空");
            NopOrderId.IsNullOrEmpty().TrueThrow("商城订单编号不能为空");
        }

        public string ToRequestUrlParam()
        {
            List<string> _params = new List<string>();
            _params.Add($"thirdOrder={this.NopOrderId}");
            _params.Add($"sku={JsonConvert.SerializeObject(this.Skus)}");
            _params.Add($"name={this.ReceiverName}");
            _params.Add($"province={this.JDProvince}");
            _params.Add($"city={this.JDCity}");
            _params.Add($"county={this.JDCounty}");
            _params.Add($"town={this.JDTown}");
            _params.Add($"address={this.Address}");
            _params.Add($"mobile={this.Mobile}");
            _params.Add($"email={this.Email}");
            _params.Add($"remark={this.Remark}");
            _params.Add($"invoiceState={this.InvoiceState}");
            _params.Add($"invoiceType={this.InvoiceType}");
            _params.Add($"selectedInvoiceTitle={this.SelectedInvoiceTitle}");
            _params.Add($"companyName={this.CompanyName}");
            _params.Add($"invoiceContent={this.InvoiceContent}");
            _params.Add($"paymentType={this.PaymentType}");
            _params.Add($"isUseBalance={this.IsUseBalance}");
            _params.Add($"submitState={this.submitState}");
            _params.Add($"doOrderPriceMode={this.DoOrderPriceMode}");
            _params.Add($"orderPriceSnap={JsonConvert.SerializeObject(this.OrderPriceSnap)}");

            return string.Join("&", _params);
        }
    }

    public class JDSubmitOrderIn_Sku
    {
        [JsonProperty(PropertyName = "skuId")]
        public long JDSkuId { get; set; }

        [JsonProperty(PropertyName = "num")]
        public int Num { get; set; }

        /// <summary>
        /// 是否需要附件
        /// </summary>
        [JsonProperty(PropertyName = "bNeedAnnex")]
        public bool bNeedAnnex => true;

        /// <summary>
        /// 是否需要增品
        /// </summary>
        [JsonProperty(PropertyName = "bNeedGift")]
        public bool bNeedGift => false;
    }

    public class JDSubmitOrderIn_PriceSnap
    {
        [JsonProperty(PropertyName = "skuId")]
        public long SkuId { get; set; }

        /// <summary>
        /// 快照价格
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public decimal PriceSnap { get; set; }
    }
}
