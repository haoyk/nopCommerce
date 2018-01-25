using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.JD
{
    /// <summary>
    /// 京东公用ClientId信息，用于获取公用Token
    /// </summary>
    [NotMapped]
    public class JDCommonClientSetting : JDClientInfo, ISettings
    {
        private new int Id { get; set; }

        public void Check()
        {
            if (this.ClientId.IsNullOrEmpty() || this.ClientSecret.IsNullOrEmpty() || this.UserName.IsNullOrEmpty() ||
                this.UserPwd.IsNullOrEmpty())
            {
                throw new NopException("未配置京东公用Client信息，请联系管理员。");
            }
        }
    }
}
