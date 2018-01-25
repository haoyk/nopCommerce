using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.JD
{
    public class JDCategory : BaseEntity
    {
        public int CategoryId { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public int CategoryClass { get; set; }
    }
}
