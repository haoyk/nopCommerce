using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDClientInfoMap : NopEntityTypeConfiguration<JDClientInfo>
    {
        public JDClientInfoMap()
        {
            this.ToTable("JDClientInfos");
            this.HasKey(p => p.Id);

            this.Property(p => p.ClientId).HasMaxLength(50).IsRequired();
            this.Property(p => p.ClientSecret).HasMaxLength(50).IsRequired();
            this.Property(p => p.UserName).HasMaxLength(50).IsRequired();
            this.Property(p => p.UserPwd).HasMaxLength(50).IsRequired();

            this.Ignore(p => p.GrantType);
        }
    }
}
