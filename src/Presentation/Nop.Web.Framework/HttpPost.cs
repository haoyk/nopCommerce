using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework
{
    public static class HttpPost
    {
        public static string Post(string url,string param)
        {
            //url.PostJsonAsync(param).;
            using (var client = new HttpClient())
            {
                var content = new StringContent(param, Encoding.UTF8, "application/x-www-form-urlencoded");//"application/json"

                var response = client.PostAsync(url, content).Result;

                string responseString = response.Content.ReadAsStringAsync().Result;

                return responseString;
            }
        }
    }
}
