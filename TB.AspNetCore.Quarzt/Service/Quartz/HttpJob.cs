using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB.AspNetCore.Quarzt.Service.Quartz
{
    public class HttpJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                using (StreamWriter sw = new StreamWriter(@"E:\httpjob.log", true, Encoding.UTF8))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                }
            });
        }
    }
}
