using Application.Common;
using Application.Common.DatabaseInformation;
using Application.Common.Logger;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManager;
using Application.Common.PathConfiguration;
using Application.Common.Settings;
using ManageStock.Builder;
using OrderTracking;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
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

            // load available database
            string path = PathManager.InstanceOf[EnumConfigurationPath.DatabaseInfo];
            List<DatabaseInfo> databases = new List<DatabaseInfo>();

            if (File.Exists(path))
            {
                DatabaseInfoSerializer.Load(path, out databases);
            }
            
            DatabaseWindow window = new DatabaseWindow(databases);
            if(window.ShowDialog() == true)
            {
                if(!DatabaseInfoSerializer.Save(path, window.Databases.ToList()))
                {
                    MessageBox.Show("Une erreur est survenue à la sauvegarde du stock. Veuillez redémarrer l'application.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                m_Launcher.Launch(window.SelectedDatabaseInfo, args.ToArray());
                m_MainWindow.View = m_Launcher.View;
                m_MainWindow.Show();
            }
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
