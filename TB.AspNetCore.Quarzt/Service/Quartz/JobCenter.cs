using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TB.AspNetCore.Data.Entity;
using TB.AspNetCore.Quarzt.Models;

namespace TB.AspNetCore.Quarzt.Service.Quartz
{
    /// <summary>
    /// 任务调度中心
    /// </summary>
    public static class JobCenter
    {
        public static ScheduleManager Manager { get; set; }
        /// <summary>
        /// 任务计划
        /// </summary>
        private static IScheduler _scheduler = null;
        public static async Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
            {
                return _scheduler;
            }
            else
            {
                ISchedulerFactory schedf = new StdSchedulerFactory();
                IScheduler sched = await schedf.GetScheduler();
                return sched;
            }
        }
        /// <summary>
        /// 添加任务计划//或者进程终止后的开启
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> StartScheduleJobAsync(ScheduleInfo scheduleInfo, bool doatonce)
        {
            try
            {
                if (scheduleInfo != null)
                {
                    if (scheduleInfo.StartRunTime == null)
                    {
                        scheduleInfo.StartRunTime = DateTime.Now;
                    }
                    DateTimeOffset startRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.StartRunTime, 1);
                    if (scheduleInfo.EndRunTime == null)
                    {
                        scheduleInfo.EndRunTime = DateTime.MaxValue.AddDays(-1);
                    }
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(scheduleInfo.EndRunTime, 1);
                    if (string.IsNullOrWhiteSpace(scheduleInfo.CronExpress)|| doatonce)
                    { 
                        DateTime dt = DateTime.Now.AddMinutes(1);
                        scheduleInfo.CronExpress = $"0 {dt.Minute} {dt.Hour} * * ?";
                    }
                    _scheduler = await GetSchedulerAsync();
                    Type type = Type.GetType(scheduleInfo.JobName, true, true);
                    IJobDetail job = JobBuilder.Create(type)
                                                .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                                                .Build();
                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                .StartAt(startRunTime)
                                                .EndAt(endRunTime)
                                                .WithIdentity(scheduleInfo.JobName, scheduleInfo.JobGroup)
                                                .WithCronSchedule(scheduleInfo.CronExpress)
                                                .Build();
                    ((CronTriggerImpl)trigger).MisfireInstruction = MisfireInstruction.CronTrigger.DoNothing;
                    //将信息写入
                    scheduleInfo.RunStatus = (int)JobStatus.Waiting;
                    Manager.SaveSchedule(scheduleInfo);
                    IList<ICronTrigger> triggers = new List<ICronTrigger> { trigger };
                    await _scheduler.ScheduleJob(job, new ReadOnlyCollection<ICronTrigger>(triggers), true);
                    if (!_scheduler.IsStarted) await _scheduler.Start();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because one already exists with this identification"))
                {
                    string ret = await ResumeScheduleJobAsync(scheduleInfo);
                }
                return false;
            }
        }

        /// <summary>
        /// 暂停指定任务计划
        /// </summary>
        /// <returns></returns>
        public static async Task<string> PauseScheduleJobAsync(ScheduleInfo scheduleInfo)
        {
            try
            {
                _scheduler = await GetSchedulerAsync();
                //使任务暂停
                await _scheduler.PauseJob(new JobKey(scheduleInfo.JobName, scheduleInfo.JobGroup));
                //更新数据库
                scheduleInfo.RunStatus = (int)JobStatus.Pause;
                Manager.UpdateScheduleStatus(scheduleInfo);
                var status = new StatusViewModel()
                {
                    Status = 0,
                    Msg = "暂停任务计划成功",
                };

                return JsonConvert.SerializeObject(status);
            }
            catch (Exception ex)
            {
                var status = new StatusViewModel()
                {
                    Status = -1,
                    Msg = "暂停任务计划失败",
                };
                return JsonConvert.SerializeObject(status);
            }
        }
        /// <summary>
        /// 恢复指定的任务计划**恢复的是暂停后的任务计划，如果是程序奔溃后 或者是进程杀死后的恢复，此方法无效
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ResumeScheduleJobAsync(ScheduleInfo scheduleInfo)
        {
            try
            {
                scheduleInfo.RunStatus = (int)JobStatus.Waiting;
                //更新model
                Manager.UpdateScheduleStatus(scheduleInfo);
                _scheduler = await GetSchedulerAsync();
                //resumejob 恢复
                await _scheduler.ResumeJob(new JobKey(scheduleInfo.JobName, scheduleInfo.JobGroup));

                var status = new StatusViewModel()
                {
                    Status = 0,
                    Msg = "恢复任务计划成功",
                };
                return JsonConvert.SerializeObject(status);
            }
            catch (Exception ex)
            {
                var status = new StatusViewModel()
                {
                    Status = -1,
                    Msg = "恢复任务计划失败",
                };
                return JsonConvert.SerializeObject(status);
            }
        }
    }


}
