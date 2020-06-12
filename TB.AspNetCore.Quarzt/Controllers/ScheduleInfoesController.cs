using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jetsun.AspNetCore.QuartzJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Simpl;
using Quartz.Xml;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TB.AspNetCore.Data.Entity;
using TB.AspNetCore.Quarzt.CompileProfile;
using TB.AspNetCore.Quarzt.Enum;
using TB.AspNetCore.Quarzt.Service.Quartz;

namespace TB.AspNetCore.Quarzt.Controllers
{
    public class ScheduleInfoesController : Controller
    {
        private readonly SchedulerDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private static IScheduler _scheduler;
        private BASE_PATHS _BASE_PATHS;
        public ScheduleInfoesController(SchedulerDbContext context, ILogger<HomeController> logger, ISchedulerFactory schedulerFactory, IOptions<BASE_PATHS> settings)
        {
            _context = context;
            JobCenter.Manager = new ScheduleManager(_context);

            _logger = logger;
            _schedulerFactory = schedulerFactory;

            _BASE_PATHS = settings.Value;
            TaskJob.BASE_PATH = _BASE_PATHS.MedicalHealth;
            TaskJobS1.BASE_PATH = _BASE_PATHS.MedicalHealthS1;
            TaskJobSY.BASE_PATH = _BASE_PATHS.MedicalHealthSY;
        }

        // GET: ScheduleInfoes
        public async Task<IActionResult> Index()
        {
            if (_scheduler == null)
                _scheduler = await JobCenter.GetSchedulerAsync();

            if (!_scheduler.IsStarted)
                await _scheduler.Start();

            if (_consumer == null)
                BuildConsumers();

            var infos = JobCenter.Manager.GetAllScheduleList().ToListAsync();
            foreach (var item in await infos)
            {
                if (item.RunStatus == (int)JobStatus.Enabled || item.RunStatus == (int)JobStatus.Waiting)
                {
                    StartTask(item.Id, false);
                }

            }
            return View(await infos);
        }

        private async Task RestartScheduler()
        {
            if (_scheduler != null && _scheduler.IsStarted)
            {
                await _scheduler.Standby();
                _scheduler = null;
            }
            XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
            _scheduler = await _schedulerFactory.GetScheduler();
            await processor.ProcessFileAndScheduleJobs("~/quartz_jobs.xml", _scheduler);
            await _scheduler.Start();
        }

        private static readonly IConnection _connection = ConnectionFactory.CreateConnection();
        private static readonly IModel _channel = _connection.CreateModel();
        private static EventingBasicConsumer _consumer = null;
        private static void BuildConsumers()
        {
            _channel.QueueDeclare(queue: "CompileMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.ExchangeDeclare(exchange: "CompileExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "CompileMessage", exchange: "CompileExchange", routingKey: string.Empty, arguments: null);
            try
            {
                if (_consumer == null) { _consumer = new EventingBasicConsumer(_channel); }
                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    using (StreamWriter sw = new StreamWriter(@"E:\receive_msg.log", true, Encoding.UTF8))
                    {
                        string tips = $"编译任务已完成:{message}";
                        string optime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
                        sw.WriteLine($"{tips} {optime}");
                        string errMsg = string.Empty;
                        string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "MessageBox.exe");
                        JobHelper.ExecBatch(path, false, false, false, $"{tips} {optime.Replace(" ", "{SPACE}")}", ref errMsg);
                    }
                };
                _channel.BasicConsume(queue: "CompileMessage",
                                     autoAck: true,
                                     consumer: _consumer);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(@"E:\err_msg.log", true, Encoding.UTF8))
                {
                    sw.WriteLine($"接收消息队列异常:{ex.Message}-{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                }
            }
        }

        private static ConnectionFactory _connectionFactory;
        private static ConnectionFactory ConnectionFactory
        {
            get
            {
                if (_connectionFactory == null)
                    _connectionFactory = new ConnectionFactory
                    {
                        HostName = "localhost",//RabbitMQ服务在本地运行
                        UserName = "guest",//用户名
                        Password = "guest"//密码 
                    };

                return _connectionFactory;
            }
        }

        // GET: ScheduleInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,JobGroup,JobName,RunStatus,CronExpress,StartRunTime,EndRunTime,NextRunTime,Token,AppId,ServiceCode,InterfaceCode,TaskDescription,DataStatus,Creator,CreateTime")] ScheduleInfo scheduleInfo)
        {
            if (ModelState.IsValid)
            {
                scheduleInfo.CreateTime = DateTime.Now;
                JobCenter.Manager.Add(scheduleInfo, true);
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleInfo);
        }

        public async void StartTask(int? id, [FromQuery] bool? doatonce)
        {
            var scheduleInfo = JobCenter.Manager.Single<ScheduleInfo>(a => a.Id == id);
            bool atonce = doatonce != null && doatonce.Value;
            await JobCenter.StartScheduleJobAsync(scheduleInfo, atonce);
        }

        public async void StopTask(int? id)
        {
            var scheduleInfo = JobCenter.Manager.Single<ScheduleInfo>(a => a.Id == id);
            await JobCenter.PauseScheduleJobAsync(scheduleInfo);
        }

        // GET: ScheduleInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var scheduleInfo = await JobCenter.Manager.DataContext.ScheduleInfo.FirstOrDefaultAsync(a => a.Id == id);
            if (scheduleInfo == null) return NotFound();

            return View(scheduleInfo);
        }

        // POST: ScheduleInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleInfo = await JobCenter.Manager.DataContext.ScheduleInfo.FindAsync(id);
            JobCenter.Manager.DataContext.ScheduleInfo.Remove(scheduleInfo);
            await JobCenter.Manager.DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
