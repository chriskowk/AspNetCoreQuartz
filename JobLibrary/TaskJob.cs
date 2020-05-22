using Quartz;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Jetsun.AspNetCore.QuartzJobs
{
    public interface IExecWithEvent
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<EventArgs> JobStarting;
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<EventArgs> JobFinished;
    }

    public abstract class TaskJobBase : IJob, IExecWithEvent
    {
        public abstract string BasePath { get; }
        public abstract string BatchFilesPath { get; }
        public abstract string JSSVCFilePath { get; }

        private ConnectionFactory _connectionFactory;
        private ConnectionFactory ConnectionFactory
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

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                JobStarting?.Invoke(this, new EventArgs());

                TFGetLatestVersion();
                BuildAll();
                RebuildDataModels();
                PushMessageQueue(this.GetType().ToString());

                JobFinished?.Invoke(this, new EventArgs());
            });
        }

        private void PushMessageQueue(string jobType)
        {
            using (var connection = ConnectionFactory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "CompileMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "CompileExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "CompileMessage", exchange: "CompileExchange", routingKey: string.Empty, arguments: null);
                    byte[] body = Encoding.UTF8.GetBytes(jobType);
                    channel.BasicPublish(exchange: "CompileExchange", routingKey: string.Empty, basicProperties: null, body: body);
                }
            }
        }

        private void TFGetLatestVersion()
        {
            string path = Path.Combine(BatchFilesPath, "TF_GET_MedicalHealth.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            string output = JobHelper.ExecBatch(path, true, false, false, string.Empty, ref errMsg);

            FileStream fs = new FileStream(string.Format(@"{0}\Log\TFGetLog{1}.txt", BatchFilesPath, DateTime.Now.ToString("yyyyMMddHHmmss")), FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(string.Format("Output: {0}\r\nErrorMsg:\r\n{1}", output, errMsg));
            sw.Close();
            fs.Close();
        }

        private void BuildAll()
        {
            string path = Path.Combine(BatchFilesPath, "全编译Upload.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            JobHelper.ExecBatch(path, true, false, false, string.Empty, ref errMsg);
        }

        public virtual void RebuildDataModels()
        {
        }

        public event EventHandler<EventArgs> JobStarting;

        public event EventHandler<EventArgs> JobFinished;
    }

    public class TaskJob : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath => BASE_PATH;
        public static string GetBatchFilePath() => Path.Combine(BASE_PATH, "BatchFiles");
        public static string GetJSSVCFilePath() => Path.Combine(BASE_PATH, "Lib");
        public override string BatchFilesPath => GetBatchFilePath();
        public override string JSSVCFilePath => GetJSSVCFilePath();
        public override void RebuildDataModels()
        {
            base.RebuildDataModels();
            string path = Path.Combine(BatchFilesPath, "tempcmd.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            JobHelper.ExecBatch(path, true, false, false, string.Empty, ref errMsg);
        }
    }

    public static class JobHelper
    {
        public static string ExecBatch(string batPath, bool useShellExecute, bool redirectStandardOutput, bool redirectStandardError, string arguments, ref string errMsg)
        {
            string outputString = string.Empty;

            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(batPath);
                ProcessStartInfo psi = new ProcessStartInfo(batPath, arguments)
                {
                    WorkingDirectory = file.Directory.FullName,
                    CreateNoWindow = false,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                    UseShellExecute = useShellExecute
                };
                pro.StartInfo = psi;
                pro.Start();
                pro.WaitForExit();

                outputString = redirectStandardOutput ? pro.StandardOutput.ReadToEnd() : string.Empty;
                errMsg = redirectStandardError ? pro.StandardError.ReadToEnd() : string.Empty;
            }
            return outputString;
        }
    }

}
