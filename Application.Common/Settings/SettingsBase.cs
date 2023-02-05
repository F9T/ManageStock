using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Application.Common.Settings
{
    public abstract class SettingsBase : INotifyPropertyChanged, ICloneable
    {
        public abstract void Default();

        public abstract void CopyTo(SettingsBase _Settings);

        public abstract object Clone();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
