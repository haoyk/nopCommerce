using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.DTO
{
    public class JDCodeNameListOut : JDOutBase
    {
        public List<JDCodeNameListOut_List> Data { get; set; }
    }

    public class JDCodeNameListOut_List
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
