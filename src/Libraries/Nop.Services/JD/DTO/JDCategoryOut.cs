using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.JD;

namespace Nop.Services.JD.DTO
{
    public class JDCategoryOut
    {
        public int CategoryId { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public int CategoryClass { get; set; }

        public JDCategory ToJdCategory()
        {
            return new JDCategory()
            {
                CategoryId = this.CategoryId,
                CategoryClass = this.CategoryClass,
                Name = this.Name,
                ParentId = this.ParentId
            };
        }
    }
}
