using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.JD
{
    /// <summary>
    /// 京东Token
    /// </summary>
    public class JDToken
    {
        public string Access_Token { get; set; }

        public DateTime Access_Token_Expires { get; set; }

        public string Refresh_Token { get; set; }

        public DateTime Refresh_Token_Expires { get; set; }

        public string UID { get; set; }

        public string Time { get; set; }

    }
}
