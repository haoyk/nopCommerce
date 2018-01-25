using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.JD.Model
{
    public class JDDictionaryResult<TKey, TValue> : JDResultBase
    {
        public Dictionary<TKey, TValue> result { get; set; }
    }
}
