using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDResultBase
    {
        public bool success { get; set; }

        public string resultMessage { get; set; }

        public string resultCode { get; set; }
    }
}
