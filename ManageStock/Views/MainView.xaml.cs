using Application.Backup;
using Application.CommandManager;
using Application.CommandManager.Collection;
using Application.CommandManager.Commands;
using Application.Common;
using Application.Common.DatabaseInformation;
using Application.Common.Logger;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Models;
using Application.Common.Notifications;
using Application.Common.PathConfiguration;
using Application.Common.PopupWindows;
using Application.Common.Settings;
using Application.Common.ViewModels;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ManageStock.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl, INotifyPropertyChanged
    {
        private SettingsSerializer<Settings> m_SettingsSerializer;
        private ViewModelBase m_SelectedView;
        private CommandManager m_CommandManager;

        public MainView()
        {
            m_SettingsSerializer = new SettingsSerializer<Settings>();
            ViewModels = new List<ViewModelBase>();

            InitializeComponent();
            DataContext = this;
        }

        public bool IsDatabaseOpened { get; set; }

        public List<ViewModelBase> ViewModels { get; set; }

        public CommandManager CommandManager
        {
            get { return m_CommandManager; }
            set
            {
                if (m_CommandManager != null)
                {
                    m_CommandManager.AfterUndo -= CommandManagerAfterUndoOrRedo;
                    m_CommandManager.AfterRedo -= CommandManagerAfterUndoOrRedo;
                }
                m_CommandManager = value;
                if (m_CommandManager != null)
                {
                    m_CommandManager.AfterUndo += CommandManagerAfterUndoOrRedo;
                    m_CommandManager.AfterRedo += CommandManagerAfterUndoOrRedo;
                }
            }
        }

        public ViewModelBase SelectedView
        {
            get
            {
                return m_SelectedView;
            }
            set
            {
                m_SelectedView = value;
                OnPropertyChanged();
            }
        }

        public Settings Settings { get; set; }

        public DatabaseInfo DatabaseInfo { get; set; }

        public CustomNotificationsManager NotificationManager { get; set; }

        private void CommandManagerAfterUndoOrRedo(object sender, CommandManagerEventArgs e)
        {
            if (e.Commands.Commands.Count > 0)
            {
                NotificationManager.SuspendNotification();

                bool? result = null;
                var groupByContext = e.Commands.Commands.GroupBy(_ => _.Context).ToDictionary(_ => _.Key, _ => _.ToList());

                var contextsFind = new List<IDatabaseModel>();
                foreach (var groupContext in groupByContext)
                {
                    var context = groupContext.Key as IDatabaseModel;
                    var commands = groupContext.Value;

                    if (context == null)
                    {
                        continue;
                    }

                    contextsFind.Add(context);

                    var groupByCommands = commands.GroupBy(_ => _.GetType()).ToDictionary(_ => _.Key, _ => _.ToList());

                    foreach (var groupCommand in groupByCommands)
                    {
                        var type = groupCommand.Key;
                        var groupCommands = groupCommand.Value;

                        bool? r = UndoRedoAction(e.CommandActionType, type, groupCommands, context);
                        if(result != false)
                        {
                            result = r;
                        }
                    }
                }

                var requestContext = contextsFind.OrderBy(_ => _.ZIndex).FirstOrDefault();
                RequestViewChange(requestContext);
                NotificationManager.ResumeNotification();

                if (result == true)
                {
                    NotificationManager.Show("Les données ont bien été rétablies", NotificationType.Success);
                }
                else
                {
                    NotificationManager.Show("Une erreur est survenue.", NotificationType.Error);
                }
            }
        }

        private bool? UndoRedoAction(EnumCommandActionType _Action, Type _ActionType, List<CommandBase> _Commands, IDatabaseModel _Context)
        {
            bool? result = true;

            if (_ActionType == typeof(PropertyCommand))
            {
                result = DataManager.Execute(EnumDatabaseAction.Update, _Context);
            }
            else if (_ActionType.GetGenericTypeDefinition() == typeof(CollectionChangedCommand<>))
            {
                foreach (var command in _Commands)
                {
                    NotifyCollectionChangedAction action = (NotifyCollectionChangedAction)command.GetType().GetProperty("Action").GetValue(command, null);

                    switch (action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            if (_Action == EnumCommandActionType.Undo)
                            {
                                DataManager.Execute(EnumDatabaseAction.Delete, _Context);
                            }
                            else
                            {
                                DataManager.Execute(EnumDatabaseAction.Insert, _Context);
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            if (_Action == EnumCommandActionType.Undo)
                            {
                                DataManager.Execute(EnumDatabaseAction.Insert, _Context);
                            }
                            else
                            {
                                DataManager.Execute(EnumDatabaseAction.Delete, _Context);
                            }
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            break;
                    }
                }
            }

            return result;
        }

        private void RequestViewChange(IDatabaseModel _Context)
        {
            if (_Context == null)
            {
                return;
            }

            var viewModel = ViewModels.FirstOrDefault(_ => _.ModelType == _Context.GetType());
            if (viewModel == null)
            {
                return;
            }

            viewModel.RequestView(_Context);
            SelectedView = viewModel;
        }

        private void SettingsButtonOnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow(Settings)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            bool? dialogResult = window.ShowDialog();

            Settings = window.Settings;

            if (dialogResult == true)
            {
                if (window.PermanentDatabaseDelete)
                {
                    BackupManager.InstanceOf.Stop();
                    DBManager.InstanceOf.Close();

                    if (!DatabaseCreator.DeleteDatabase(DatabaseInfo))
                    {
                        MessageBox.Show("Impossible de supprimer le stock.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        ApplicationLogger.InstanceOf.Write($"Impossible de supprimer la base de donnée {DatabaseInfo?.ConnectionString}");
                        System.Windows.Application.Current.Shutdown(0);
                        return;
                    }

                    // clear backup
                    BackupManager.InstanceOf.ClearAllBackup();

                    MessageBox.Show("Le stock a bien été supprimé. Veuillez redémarrer l'application.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Windows.Application.Current.Shutdown(0);
                    return;
                }
                else
                {
                    bool result = m_SettingsSerializer.Save(PathManager.InstanceOf[EnumConfigurationPath.Settings], Settings);
                    if (Settings.EnabledNotifications)
                    {
                        if (result)
                        {
                            NotificationManager.Show("Les paramètres ont été sauvegardés avec succès", NotificationType.Success);
                        }
                        else
                        {
                            NotificationManager.Show("Une erreur est survenue durant la sauvegarde des paramètres", NotificationType.Warning);
                        }
                    }

                    if (window.NeedRestart)
                    {
                        Restart();
                    }
                }
            }
        }

        public void Restart()
        {
            ConfirmationPopup popup = new ConfirmationPopup("L'application va être redémarrée.", "");
            popup.Owner = System.Windows.Application.Current.MainWindow;
            popup.YesButtonText = "Ok";
            popup.ShowNoButton = false;
            popup.ShowCancelButton = false;
            popup.ShowDialog();

            var executingPath = Assembly.GetEntryAssembly().Location;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                // wait 5 seconds before restart the application
                Arguments = "/C ping 127.0.0.1 -n 2 && \"" + executingPath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(startInfo);
            System.Windows.Application.Current.Shutdown(0);
        }

        private void UndoButtonClick(object sender, RoutedEventArgs e)
        {
            ConfirmationPopup popup = new ConfirmationPopup("Êtes-vous sûr de vouloir annuler la dernière action ?");
            popup.Owner = System.Windows.Application.Current.MainWindow;
            if(popup.ShowDialog() == true)
            {
                if (popup.Result == EnumPopupResult.Yes)
                {
                    SelectedView.Undo();
                }
            }
        }

        private void RedoButtonClick(object sender, RoutedEventArgs e)
        {
            ConfirmationPopup popup = new ConfirmationPopup("Êtes-vous sûr de vouloir refaire la dernière action ?");
            popup.Owner = System.Windows.Application.Current.MainWindow;
            if (popup.ShowDialog() == true)
            {
                if(popup.Result == EnumPopupResult.Yes)
                {
                    SelectedView.Redo();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
