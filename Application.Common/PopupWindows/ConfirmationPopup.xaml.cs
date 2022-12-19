using Application.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Application.Common.PopupWindows
{
    /// <summary>
    /// Interaction logic for ConfirmationPopup.xaml
    /// </summary>
    public partial class ConfirmationPopup : CustomWindow, INotifyPropertyChanged
    {
        private bool m_ShowYesButton;
        private bool m_ShowNoButton;
        private bool m_ShowCancelButton;
        private bool m_ManualClosing = true;
        private string m_YesButtonText;

        public ConfirmationPopup(string _Message, string _Title = "")
        {
            m_YesButtonText = "Oui";
            Message = _Message;
            WindowTitle = _Title;
            ShowCancelButton = true;
            ShowNoButton = true;
            ShowYesButton = true;

            InitializeComponent();
            DataContext = this;
        }

        public EnumPopupResult Result { get; set; }

        public string WindowTitle { get; set; }

        public string Message { get; set; }

        public string YesButtonText
        {
            get { return m_YesButtonText; }
            set
            {
                m_YesButtonText = value;
                OnPropertyChanged();
            }
        }

        public bool ShowYesButton
        {
            get => m_ShowYesButton;
            set
            {
                m_ShowYesButton = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNoButton
        {
            get => m_ShowNoButton;
            set
            {
                m_ShowNoButton = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCancelButton
        {
            get => m_ShowCancelButton;
            set
            {
                m_ShowCancelButton = value;
                OnPropertyChanged();
            }
        }

        private void CancelButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            Result = EnumPopupResult.Cancel;
            DialogResult = true;
        }

        private void NoButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            Result = EnumPopupResult.No;
            DialogResult = true;
        }

        private void YesButtonOnClick(object sender, RoutedEventArgs e)
        {
            m_ManualClosing = true;
            Result = EnumPopupResult.Yes;
            DialogResult = true;
        }

        private void ConfirmationPopupOnClosing(object sender, CancelEventArgs e)
        {
            if (!m_ManualClosing)
            {
                Result = EnumPopupResult.Cancel;
                DialogResult = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
