using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Application.Common.Settings
{
    public abstract class SettingsBase : INotifyPropertyChanged, ICloneable
    {
        private string m_DatabasePath;

        public string DatabasePath
        {
            get => m_DatabasePath;
            set
            {
                m_DatabasePath = value;
                OnPropertyChanged();
            }
        }

        public virtual void Default()
        {
            m_DatabasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ManageStock.db");
        }

        public virtual void CopyTo(SettingsBase _Settings)
        {
            m_DatabasePath = _Settings.DatabasePath;
        }

        public abstract object Clone();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
