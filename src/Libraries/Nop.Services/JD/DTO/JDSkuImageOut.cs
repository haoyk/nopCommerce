using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDSkuImageOut
    {
        public string Path { get; set; }

        public bool IsPrimary { get; set; }

        public int OrderSort { get; set; }
    }
}
