using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.JD
{
    public class JDSkuImage : BaseEntity
    {
        public string Path { get; set; }

        public bool IsPrimary { get; set; }

        public int OrderSort { get; set; }
    }
}
