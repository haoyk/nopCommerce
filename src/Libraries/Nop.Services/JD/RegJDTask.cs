using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.JD.Task;
using Nop.Services.Tasks;

namespace Nop.Services.JD
{
    /// <summary>
    /// 网站应用启动时,检查并注册京东相关Task
    /// </summary>
    public class RegJDTask : IStartupTask
    {
        private IScheduleTaskService _scheduleTaskService;
        private ITypeFinder _typeFinder;

        /// <summary>
        /// 重构器必须无参
        /// </summary>
        public RegJDTask()
        {
            _scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            _typeFinder = EngineContext.Current.Resolve<ITypeFinder>(); ;
        }

        public int Order => 9;

        public void Execute()
        {
            //获取所有京东Task
            var jdTasks = _typeFinder.FindClassesOfType<IJDTask>();

            //系统已注册的Task
            var allTask = _scheduleTaskService.GetAllTasks(true);

            //未注册计划任务的Task
            var notRegJDTask = jdTasks.Where(t => allTask.Select(p => p.Type).Contains(t.AssemblyQualifiedName) == false);

            //注册
            notRegJDTask.Select(p =>
            {
                var task = (IJDTask)Activator.CreateInstance(p);

                return new ScheduleTask()
                {
                    Name = task.TaskName,
                    Seconds = task.Minutes * 60,
                    Type = p.AssemblyQualifiedName,
                    Enabled = true,
                    StopOnError = task.StopOnError
                };
            }).ToList().ForEach(task =>
            {
                _scheduleTaskService.InsertTask(task);
            });

        }
    }
}
