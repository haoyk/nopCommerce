using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;

namespace Nop.Services.Http
{
    public class HttpService: IHttpService
    {
        private readonly ILogger _log;
        public HttpService(ILogger log)
        {
            _log = log;
        }

        public string Post(string url, string param)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(param, Encoding.UTF8, "application/x-www-form-urlencoded");//"application/json"

                    var response = client.PostAsync(url, content).Result;

                    string responseString = response.Content.ReadAsStringAsync().Result;

                    return responseString;
                }
            }
            catch (Exception e)
            {
                _log.InsertLog(LogLevel.Error, "POST请求失败", $"URL:{url}, Param:{param}, ErrorMessage:{e.Message}");
                return "";
            }
        }
    }
}
