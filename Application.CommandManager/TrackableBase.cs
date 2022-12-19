using Application.CommandManager.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Application.CommandManager
{
    public abstract class TrackableBase : INotifyPropertyChanged
    {
        protected CommandManager m_CommandManager;

        public virtual void InitializeTrackable(CommandManager _CommandManager) => m_CommandManager = _CommandManager;

        public CommandManager CommandManager
        {
            get => m_CommandManager;
            set => m_CommandManager = value;
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string _PropertyName, object _OldValue, object _NewValue)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }

            if (m_CommandManager == null || _OldValue.Equals(_NewValue))
            {
                return;
            }

            m_CommandManager.AddCommand(new PropertyCommand(this, _PropertyName, _OldValue, _NewValue));
        }

        protected virtual void PerformPropertyChangeWithoutCommand(string _PropertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }

            propertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
        }
    }
}
