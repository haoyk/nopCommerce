using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDCommonJsonMap : NopEntityTypeConfiguration<JDCommonJson>
    {
        public JDCommonJsonMap()
        {
            this.ToTable("JDCommonJson");
            this.HasKey(bp => bp.Id);

            this.Property(p => p.Parent).IsRequired().HasMaxLength(50);
            this.Property(p => p.Json).IsRequired().HasMaxLength(null);
            this.Property(p => p.JsonSchemaType).IsRequired().HasMaxLength(50);
            
        }
    }
}
