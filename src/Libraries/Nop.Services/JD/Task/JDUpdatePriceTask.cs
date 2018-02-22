using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Services.JD.Task
{
    public class JDUpdatePriceTask : IJDTask
    {
        public JDUpdatePriceTask()
        {
        }

        private ILogger _log => EngineContext.Current.Resolve<ILogger>();

        string IJDTask.TaskName => "京东更新价格";

        int IJDTask.Minutes => 60;

        bool IJDTask.StopOnError => false;

        void ITask.Execute()
        {
            _log.Information("计划任务-更新价格开始");


            _log.Information("计划任务-更新价格结束");
        }
    }
}
