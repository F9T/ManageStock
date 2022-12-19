using Application.Backup;
using Application.CommandManager;
using Application.Common;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.PathConfiguration;
using Application.Common.Settings;
using Application.Common.ViewModels;
using ManageStock.ViewModels;
using ManageStock.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ManageStock.Builder
{
    public class ManageStockBuilder : AppBuilderBase
    {
        private MainView m_MainView;
        private FileSystemWatcher m_FileSystemWatcher;

        public static bool IsLocked { get; private set; }

        public override UserControl View => m_MainView;

        public override void Launch(object[] _Args)
        {
            m_MainView = new MainView();

            base.Launch(_Args);

            if (_Args.Length == 1)
            {
                if (_Args[0] != null && _Args[0].ToString() == "unlock")
                {
                    DBManager.InstanceOf.SetLock(false);
                }
            }

            m_MainView.Settings = Settings;
            Settings.PropertyChanged += SettingsOnPropertyChanged;

            m_MainView.NotificationManager = CustomNotificationsManager;

            InitializeLock();
            InitializePath();
            InitializeViewModels();
        }

        public override void Shutdown(int _ExitCode)
        {
            if (!IsLocked)
            {
                DBManager.InstanceOf.SetLock(false);
            }

            base.Shutdown(_ExitCode);
        }

        private void InitializePath()
        {
            var settingsDir = Path.GetDirectoryName(PathManager.InstanceOf[EnumConfigurationPath.Settings]);

            if (!Directory.Exists(settingsDir))
            {
                try
                {
                    Directory.CreateDirectory(settingsDir);
                }
                catch (Exception)
                {
                    CustomNotificationsManager.Show($"Une erreur est survenue dans la création du dossier des paramètres de l'application ({settingsDir}).", Notification.Wpf.NotificationType.Error);
                    Shutdown(-1);
                    return;
                }
            }

            var logDir = Path.GetDirectoryName(PathManager.InstanceOf[EnumConfigurationPath.Logs]);

            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception)
                {
                    CustomNotificationsManager.Show($"Une erreur est survenue dans la création du dossier des logs de l'application ({logDir}).", Notification.Wpf.NotificationType.Error);
                    Shutdown(-1);
                    return;
                }
            }
        }

        private void InitializeBackup()
        {
            var dirBackup = Path.Combine(Path.GetDirectoryName(PathManager.InstanceOf[EnumConfigurationPath.Database]), "Backup");
            BackupInfo backupInfo = new BackupInfo(dirBackup, 3600, 120);

            BackupManager.InstanceOf.AddBackup(PathManager.InstanceOf[EnumConfigurationPath.Database], backupInfo);
            BackupManager.InstanceOf.Start();
        }

        private void InitializeWatcher()
        {
            var settingsDir = Path.GetDirectoryName(PathManager.InstanceOf[EnumConfigurationPath.Database]);

            m_FileSystemWatcher = new FileSystemWatcher(settingsDir)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                Filter = "*.db",
                NotifyFilter = NotifyFilters.LastWrite
            };
            m_FileSystemWatcher.Changed += DatabaseWatcherChanged;
        }

        private void DatabaseWatcherChanged(object sender, FileSystemEventArgs e)
        {
            foreach (ViewModelBase viewModel in m_MainView.ViewModels)
            {
                if (viewModel == m_MainView.SelectedView)
                {
                    viewModel.LoadingInfo.IsLoading = true;
                    viewModel.LoadingInfo.Text = "Rafraichissement des données...";
                    viewModel.LoadingInfo.IsIndeterminate = true;
                }
                Task.Factory.StartNew(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        viewModel.Reload();
                        if (viewModel == m_MainView.SelectedView)
                        {
                            viewModel.LoadingInfo.IsLoading = false;
                            viewModel.LoadingInfo.Text = "";
                        }
                    }));
                });
            }
        }

        private void InitializeLock()
        {
            if (DBManager.InstanceOf.IsLocked())
            {
                IsLocked = true;
                InitializeWatcher();
            }
            else
            {
                DBManager.InstanceOf.SetLock(true);
                IsLocked = false;
                InitializeBackup();
            }
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.EnabledNotifications))
            {
                if (m_MainView.Settings.EnabledNotifications)
                {
                    m_MainView.NotificationManager.EnabledNotification();
                }
                else
                {
                    m_MainView.NotificationManager.DisableNotification();
                }
            }
        }


        private void InitializeViewModels()
        {
            CommandManager commandManager = new CommandManager();

            List<ViewModelBase> viewModels = new List<ViewModelBase>();

            ArticleViewModel articleViewModel = new ArticleViewModel(commandManager);
            articleViewModel.ChangeViewRequest += ViewModelChangeViewRequest;
            viewModels.Add(articleViewModel);

            ProviderViewModel providerViewModel = new ProviderViewModel(commandManager);
            viewModels.Add(providerViewModel);

            HistoryViewModel historyViewModel = new HistoryViewModel(commandManager);
            historyViewModel.ChangeViewRequest += ViewModelChangeViewRequest;
            viewModels.Add(historyViewModel);

            CurrencyViewModel currencyViewModel = new CurrencyViewModel(commandManager);
            viewModels.Add(currencyViewModel);

            // initialize
            foreach (ViewModelBase viewModel in viewModels)
            {
                viewModel.Initialize(CustomNotificationsManager);
                m_MainView.ViewModels.Add(viewModel);
            }

            m_MainView.CommandManager = commandManager;
        }

        private void ViewModelChangeViewRequest(object sender, ViewRequestEventArgs args)
        {
            if (args.Item != null)
            {
                var viewModel = m_MainView.ViewModels.FirstOrDefault(_ => _.GetType() == args.ViewType);

                if (viewModel != null)
                {
                    viewModel.RequestView(args.Item);
                    m_MainView.SelectedView = viewModel;
                }
            }
        }

        public override void Dispose()
        {
            foreach (ViewModelBase viewModel in m_MainView.ViewModels)
            {
                viewModel.ChangeViewRequest -= ViewModelChangeViewRequest;
                viewModel.Dispose();
            }

            if (m_FileSystemWatcher != null)
            {
                m_FileSystemWatcher.Changed -= DatabaseWatcherChanged;
            }

            BackupManager.InstanceOf.Stop();
            base.Dispose();
        }
    }
}
