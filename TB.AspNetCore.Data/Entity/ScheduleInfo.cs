﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TB.AspNetCore.Data.Entity
{
    public class ScheduleInfo
    {
        public int Id { get; set; }
        [DisplayName("作业分组")]
        public string JobGroup { get; set; }
        [DisplayName("作业名称")]
        public string JobName { get; set; }
        [DisplayName("状态")]
        public int RunStatus { get; set; }
        [DisplayName("Cron表达式")]
        public string CronExpress { get; set; }
        [DisplayName("开始运行时间")]
        public DateTime? StartRunTime { get; set; }
        [DisplayName("结束运行时间")]
        public DateTime? EndRunTime { get; set; }
        [DisplayName("下次运行时间")]
        public DateTime? NextRunTime { get; set; }
        [DisplayName("令牌")]
        public string Token { get; set; }
        [DisplayName("应用ID")]
        public string AppId { get; set; }
        [DisplayName("服务编码")]
        public string ServiceCode { get; set; }
        [DisplayName("接口编码")]
        public string InterfaceCode { get; set; }
        [DisplayName("任务描述")]
        public string TaskDescription { get; set; }
        [DisplayName("数据状态")]
        public int? DataStatus { get; set; }
        [DisplayName("创建人")]
        public string Creator { get; set; }
        [DisplayName("创建时间")]
        public DateTime? CreateTime { get; set; }
    }
}
