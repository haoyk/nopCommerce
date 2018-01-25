using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Services.JD.Model
{
    public class JDCategoryResult : JDResultBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JDCategoryResult_Map result { get; set; }
    }


    public class JDCategoryResult_Map
    {
        public int totalRows { get; set; }

        public int pageNo { get; set; }

        public int pageSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JDCategoryResult_Detail> categorys { get; set; }
    }

    public class JDCategoryResult_Detail
    {
        public int catId { get; set; }

        public int parentId { get; set; }

        public string name { get; set; }

        public int catClass { get; set; }

        /// <summary>
        /// 1：有效；0：无效；
        /// </summary>
        public int state { get; set; }
    }
}
