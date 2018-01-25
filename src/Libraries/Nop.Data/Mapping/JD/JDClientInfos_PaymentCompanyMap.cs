using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDClientInfos_PaymentCompanyMap : NopEntityTypeConfiguration<JDClientInfos_PaymentCompany>
    {
        public JDClientInfos_PaymentCompanyMap()
        {
            this.ToTable("JDClientInfos_PaymentCompany_Mapping");
            this.HasKey(p => p.Id);
        }
    }
}
