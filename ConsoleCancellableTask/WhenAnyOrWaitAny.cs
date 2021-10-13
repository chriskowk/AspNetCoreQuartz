using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCancellableTask
{
    public static class WhenAnyOrWaitAnyHelper
    {
        static readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public static async void DoTestAsync(WhenAnyOrWaitAnyEnum option)
        {
            Task completedTask = null;
            int index = -1;
            CancellationToken token = _cts.Token;
            var ca = TcpClientConnectAsync("192.168.1.88", 9999, token);
            var cb = AuditAsync("ping pong", token);
            Task[] tasks = new Task[] { ca, cb };
            switch (option)
            {
                case WhenAnyOrWaitAnyEnum.WHEN_ANY:
                    completedTask = await Task.WhenAny(tasks);
                    break;
                case WhenAnyOrWaitAnyEnum.WAIT_ANY:
                    index = Task.WaitAny(tasks, token);
                    break;
            }
            _cts.Cancel();

            if (completedTask == null && index != -1)
                completedTask = tasks[index];

            if (completedTask != null)
            {
                Console.WriteLine($"index={index}; completedTask.Id={completedTask.Id}; ");
                if (completedTask == cb)
                    Console.WriteLine($"completedTask.Result={cb.Result}; ");
            }

            Console.WriteLine($"TcpClientConnectAsync.Id={ca.Id} | TcpClientConnectAsync.Status={ca.Status}; AuditAsync.Id={cb.Id} | AuditAsync.Status={cb.Status}");
            if (ca.IsFaulted || !ca.IsCompleted)
                Console.WriteLine("Connection failed due to timeout or exception");
            else
                Console.WriteLine("Connected");
            Console.WriteLine("\r\nChecking all tasks status ...");
            foreach (var t in tasks)
            {
                Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
            }
        }

        static async Task TcpClientConnectAsync(string host, int port, CancellationToken cancellationToken)
        {
            using (TcpClient client = new TcpClient())
            {
                if (cancellationToken.IsCancellationRequested) return;
                await client.ConnectAsync(host, port);
            };
        }

        static async Task<string> AuditAsync(string para, CancellationToken cancellationToken) =>
            await Task.Run(() =>
            {
                string ret = "AuditAsync start:";
                Thread.Sleep(10000); //30秒
                if (cancellationToken.IsCancellationRequested) return ret;

                Console.WriteLine($"{ret}-{para}");
                return $"{ret}-{para}";
            });
    }

    public enum WhenAnyOrWaitAnyEnum
    {
        WHEN_ANY = 0,
        WAIT_ANY = 1
    }
}
