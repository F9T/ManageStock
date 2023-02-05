namespace Application.Common.Settings
{
    public class Settings : SettingsBase
    {
        private bool m_EnabledNotifications;
        private bool m_EnabledEmailNotifications;
        private string m_EmailNotifications;

        public bool EnabledNotifications
        {
            get => m_EnabledNotifications;
            set
            {
                m_EnabledNotifications = value;
                OnPropertyChanged();
            }
        }

        public bool EnabledEmailNotifications
        {
            get => m_EnabledEmailNotifications;
            set
            {
                m_EnabledEmailNotifications = value;
                OnPropertyChanged();
            }
        }

        public string EmailNotifications
        {
            get => m_EmailNotifications;
            set
            {
                m_EmailNotifications = value;
                OnPropertyChanged();
            }
        }

        public override void Default()
        {
            m_EnabledEmailNotifications = false;
            m_EmailNotifications = "info@hfmb.ch";
            m_EnabledNotifications = true;
        }

        public override void CopyTo(SettingsBase _Settings)
        {
            if (_Settings is Settings settings)
            {
                m_EnabledNotifications = settings.EnabledNotifications;
                m_EnabledEmailNotifications = settings.EnabledEmailNotifications;
                m_EmailNotifications = settings.EmailNotifications;
            }
        }

        public override object Clone()
        {
            Settings settings = (Settings)MemberwiseClone();
            return settings;
        }
    }
}
