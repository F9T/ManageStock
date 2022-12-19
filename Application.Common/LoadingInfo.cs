using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Application.Common
{
    public class LoadingInfo : INotifyPropertyChanged
    {
        private bool m_IsLoading;
        private bool m_IsIndeterminate;
        private string m_Text;
        private double m_Value;

        public bool IsLoading
        {
            get { return m_IsLoading; }
            set
            {
                m_IsLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get { return m_IsIndeterminate; }
            set
            {
                m_IsIndeterminate = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                OnPropertyChanged();
            }
        }
        public double Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
