using Notification.Wpf;

namespace Application.Common.Notifications
{
    public class CustomNotificationsManager : NotificationManager
    {
        private bool m_IsEnabled;
        private bool m_ReportLastStateIsEnabled;

        public CustomNotificationsManager()
        {
            m_IsEnabled = true;
            m_ReportLastStateIsEnabled = true;
        }

        public void EnabledNotification()
        {
            m_IsEnabled = true;
        }

        public void DisableNotification()
        {
            m_IsEnabled = false;
        }

        public void SuspendNotification()
        {
            m_ReportLastStateIsEnabled = m_IsEnabled;
            m_IsEnabled = false;
        }

        public void ResumeNotification()
        {
            m_IsEnabled = m_ReportLastStateIsEnabled;
        }

        public void Show(string _Title, string _Message, NotificationType _NotificationType)
        {
            if (m_IsEnabled)
            {
                base.Show(_Title, _Message, _NotificationType);
            }
        }

        public void Show(string _Message, NotificationType _NotificationType)
        {
            if (m_IsEnabled)
            {
                base.Show(_Message, _NotificationType);
            }
        }
    }
}
