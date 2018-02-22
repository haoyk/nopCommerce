using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Tasks;

namespace Nop.Services.JD.Task
{
    /// <summary>
    /// 京东Task接口
    /// </summary>
    public interface IJDTask: ITask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        string TaskName { get; }

        /// <summary>
        /// 间隔时间(分钟)
        /// </summary>
        int Minutes { get; }

        /// <summary>
        /// 发生错误时停止
        /// </summary>
        bool StopOnError { get; }
    }
}
