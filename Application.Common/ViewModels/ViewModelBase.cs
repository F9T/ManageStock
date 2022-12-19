using Application.Common.Models;
using Application.Common.Models.Items;
using Application.Common.Notifications;
using Notification.Wpf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Application.Common.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected CustomNotificationsManager m_NotificationManager;

        public event EventHandler<ViewRequestEventArgs> ChangeViewRequest;

        protected ViewModelBase(CommandManager.CommandManager _CommandManager = null)
        {
            CommandManager = _CommandManager;
            LoadingInfo = new LoadingInfo();
        }

        public virtual void Initialize(CustomNotificationsManager _NotificationManager)
        {
            m_NotificationManager = _NotificationManager;
        }

        public abstract void SelectItem(ItemBase itemBase);

        public CustomNotificationsManager GetNotificationManager()
        {
            return m_NotificationManager;
        }

        public void NotifyError(string _Message, string _Title = null)
        {
            Notify(_Title, _Message, NotificationType.Error);
        }

        public void NotifyWarning(string _Message, string _Title = null)
        {
            Notify(_Title, _Message, NotificationType.Warning);
        }

        public void NotifySucess(string _Message, string _Title = null)
        {
            Notify(_Title, _Message, NotificationType.Success);
        }

        private void Notify(string _Title, string _Message, NotificationType _NotificationType)
        {
            if (!string.IsNullOrEmpty(_Title))
            {
                m_NotificationManager?.Show(_Title, _Message, _NotificationType);
            }
            else
            {
                m_NotificationManager?.Show(_Message, _NotificationType);
            }
        }

        public LoadingInfo LoadingInfo { get; set; }

        public CommandManager.CommandManager CommandManager { get; set; }

        public string Header { get; protected set; }

        public string TemplateName { get; protected set; }

        public abstract Type ModelType { get; }

        public abstract void RequestView(IDatabaseModel _Item);

        public void Redo()
        {
            CommandManager?.Redo();
        }

        public void Undo()
        {
            CommandManager?.Undo();
        }

        public abstract void Reload();

        public abstract void Dispose();

        public void OnChangeViewRequest(ViewRequestEventArgs _EventArgs)
        {
            ChangeViewRequest?.Invoke(this, _EventArgs);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
