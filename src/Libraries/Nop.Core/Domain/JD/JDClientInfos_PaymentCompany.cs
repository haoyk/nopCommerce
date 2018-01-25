using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Infrastructure;

namespace Nop.Core.Domain.JD
{
    public class JDClientInfos_PaymentCompany : BaseEntity
    {
        private readonly IRepository<JDClientInfo> _jdClientInfoRep;

        public JDClientInfos_PaymentCompany()
        {
            _jdClientInfoRep = EngineContext.Current.Resolve<IRepository<JDClientInfo>>();
        }

        /// <summary>
        /// 发薪公司编码
        /// </summary>
        public int PaymentComId { get; set; }

        /// <summary>
        /// 京东Clint信息表主键
        /// </summary>
        public int JDClientInfoId { get; set; }

        private JDClientInfo _JDClientInfo;
        /// <summary>
        /// 京东Client信息
        /// </summary>
        public virtual JDClientInfo JDClientInfo
        {
            get
            {
                if (_JDClientInfo == null)
                {
                    _JDClientInfo = _jdClientInfoRep.Table.FirstOrDefault(p => p.Id == this.JDClientInfoId);
                }
                return _JDClientInfo;
            }
        }
    }
}
