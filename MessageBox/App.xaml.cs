using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Jetsun.AspNetCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Message;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string usage = "用法示例： MessageBox.exe 提示信息1 提示信息2 提示信息3 ......\r\n说明：提示信息若中间有空格请使用{SPACE}替换，因为空格是多个命令行参数的分隔符。";
            string msg = string.Empty;
            for (int i = 0; i < e.Args.Length; i++)
            {
                msg += e.Args[i] + "\r\n";
            }
            msg = msg.Replace("{SPACE}", " ");
            Message = string.IsNullOrWhiteSpace(msg) ? usage : msg;
        }
    }
}
