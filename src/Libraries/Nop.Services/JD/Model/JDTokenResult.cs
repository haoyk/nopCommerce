using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.JD;

namespace Nop.Services.JD.Model
{
    public class JDTokenResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDTokenResult_Detail result { get; set; }

        /// <summary>
        /// 转为JDToken类型
        /// </summary>
        /// <returns></returns>
        public JDToken ToJdToken()
        {
            return new JDToken()
            {
                Access_Token = this.result.access_token,
                Access_Token_Expires = DateTime.Now.AddSeconds(this.result.expires_in),
                Refresh_Token = this.result.refresh_token,
                Refresh_Token_Expires = UnixToDateTime(this.result.refresh_token_expires.ToString()),
                Time = this.result.time,
                UID = this.result.uid
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unix">unix毫秒</param>
        /// <returns></returns>
        private DateTime UnixToDateTime(string unix)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(unix + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }

    public class JDTokenResult_Detail
    {
        public string uid { get; set; }

        public string time { get; set; }

        public long refresh_token_expires { get; set; }

        public long expires_in { get; set; }

        public string refresh_token { get; set; }

        public string access_token { get; set; }
    }
}
