using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Data.Mapping.JD
{
    public class JDAddrMap : NopEntityTypeConfiguration<JDAddr>
    {
        public JDAddrMap()
        {
            this.ToTable("JDAddr");
            this.HasKey(bp => bp.Id);

            this.Property(p => p.Code).IsRequired();
            this.Property(p => p.Parent).IsRequired();
            this.Property(p => p.Name).IsRequired().HasMaxLength(50);
            //this.Property(bp => bp.Title).IsRequired();
            //this.Property(bp => bp.Body).IsRequired();
            //this.Property(bp => bp.MetaKeywords).HasMaxLength(400);
            //this.Property(bp => bp.MetaTitle).HasMaxLength(400);

            //this.HasRequired(bp => bp.Language)
            //    .WithMany()
            //    .HasForeignKey(bp => bp.LanguageId).WillCascadeOnDelete(true);
        }
    }
}
