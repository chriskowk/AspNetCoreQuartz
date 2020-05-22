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
            string msg = string.Empty;
            for (int i = 0; i < e.Args.Length; i++)
            {
                msg += e.Args[i] + "\r\n";
            }
            msg = msg.Replace("{SPACE}", " ");
            Message = msg;
        }
    }
}
