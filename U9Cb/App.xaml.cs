using System;
using System.Windows;
using System.Threading;
namespace U9Cb
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 设置程序单例运行
        /// </summary>
        private static Mutex mutex;
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new Mutex(true, "MyApp", out ret);
            if (!ret)
            {
                Environment.Exit(0);
            }
        }
    }
}
