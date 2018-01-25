using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.JD
{
    public class JDAddr: BaseEntity
    {
        public int Code { get; set; }

        public int Parent { get; set; }

        public string Name { get; set; }
    }
}
