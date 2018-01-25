using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace Nop.Core.Domain.JD
{
    public class JDCommonJson : BaseEntity
    {
        public string JsonSchemaType { get; set; }

        public string Json { get; set; }

        public string Parent { get; set; }
    }
}
