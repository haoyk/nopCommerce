using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDSkuDetailMap : NopEntityTypeConfiguration<JDSkuDetail>
    {
        public JDSkuDetailMap()
        {
            this.ToTable("JDSkuDetails");
            this.HasKey(bp => bp.Id);
        }
    }
}
