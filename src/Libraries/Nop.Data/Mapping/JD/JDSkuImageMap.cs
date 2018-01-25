using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDSkuImageMap:NopEntityTypeConfiguration<JDSkuImage>
    {
        public JDSkuImageMap()
        {
            this.ToTable("JDSkuImages");
            this.HasKey(p => p.Id);
        }
    }
}
