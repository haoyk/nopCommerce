using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.JD
{
    public class JDClientInfo : BaseEntity
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string UserName { get; set; }

        public string UserPwd { get; set; }

        public string GrantType => "access_token";
    }
}
