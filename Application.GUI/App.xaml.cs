using Application.Common;
using Application.Common.Logger;
using ManageStock.Builder;
using OrderTracking;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Application.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private MainWindow m_MainWindow;
        private AppBuilderBase m_Launcher;

        private Dictionary<int, AppBuilderBase> appBuilders = new Dictionary<int, AppBuilderBase>
        {
            { 1, new ManageStockBuilder() },
            { 2, new OrderTrackingBuilder() }
        };

        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            m_MainWindow = new MainWindow();

            int appNumber = 1;

            if (e.Args.Length > 0)
            {
                int.TryParse(e.Args[0], out appNumber);
            }

            if (appBuilders.ContainsKey(appNumber))
            {
                m_Launcher = appBuilders[appNumber];
            }
            else
            {
                m_Launcher = appBuilders[1];
            }

            List<object> args = new List<object>();
            if(e.Args.Length > 1)
            {
                foreach (var arg in e.Args.Skip(1))
                {
                    args.Add(arg);
                }
            }
            m_Launcher.Launch(args.ToArray());
            m_MainWindow.View = m_Launcher.View;

            m_MainWindow.Show();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                ApplicationLogger.InstanceOf.Write($"UnhandledException : {e.Exception?.Message} {e.Exception?.StackTrace}");
            }
        }

        private void AppOnExit(object sender, ExitEventArgs e)
        {
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            m_Launcher.Shutdown(e.ApplicationExitCode);
        }
    }
}
