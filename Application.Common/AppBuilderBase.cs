using Application.Common.DatabaseInformation;
using Application.Common.Logger;
using Application.Common.Managers;
using Application.Common.Managers.DatabaseManager;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Notifications;
using Application.Common.PathConfiguration;
using Application.Common.Settings;
using Notification.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace Application.Common
{
    public abstract class AppBuilderBase : IDisposable
    {
        protected bool m_DatabaseIsOpened;

        protected CustomNotificationsManager CustomNotificationsManager { get; private set; }

        protected SettingsSerializer<Settings.Settings> SettingsSerializer { get; private set; }

        protected Settings.Settings Settings { get; private set; }

        public abstract UserControl View { get; }

        public DatabaseInfo DatabaseInfo { get; set; }

        public virtual void Launch(DatabaseInfo _DatabaseInfo, object[] _Args = null)
        {
            DatabaseInfo = _DatabaseInfo;
            SettingsSerializer = new SettingsSerializer<Settings.Settings>();

            CustomNotificationsManager = new CustomNotificationsManager();
            DataManager.SetNotificationManager(CustomNotificationsManager);

            Settings = LoadSettings();

            InitializeLogger();

            InitializeDatabase();
        }

        public virtual void Shutdown(int _ExitCode)
        {
            DBManager.InstanceOf.Close();
        }

        private Settings.Settings LoadSettings()
        {
            string settingsPath = PathManager.InstanceOf[EnumConfigurationPath.Settings];

            Settings.Settings settings = null;
            if (File.Exists(settingsPath))
            {
                SettingsSerializer.Load(settingsPath, out settings);
            }

            if (settings == null)
            {
                settings = new Settings.Settings();
                settings.Default();

                SettingsSerializer.Save(settingsPath, settings);
            }

            return settings;
        }

        private void InitializeLogger()
        {
            ApplicationLogger.InstanceOf.SetLoggerPath(PathManager.InstanceOf[EnumConfigurationPath.Logs]);
        }

        protected virtual void InitializeDatabase()
        {
            PathManager.InstanceOf[EnumConfigurationPath.Database] = DatabaseInfo.ConnectionString;

            // open database
            m_DatabaseIsOpened = DBManager.InstanceOf.Open(DatabaseInfo.Type, DatabaseInfo.ConnectionString);
            if (!m_DatabaseIsOpened)
            {
                ApplicationLogger.InstanceOf.Write("InitializeDatabase() : Impossible de se connecter à la base de données!");
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
