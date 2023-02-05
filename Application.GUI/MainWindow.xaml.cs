using Application.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Application.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomWindow, INotifyPropertyChanged
    {
        private UserControl m_View;

        public MainWindow()
        {
            InitializeComponent();
        }

        public UserControl View 
        { 
            get => m_View; 
            set 
            { 
                m_View = value; 
                OnPropertyChanged(); 
            } 
        }


        public override void Dispose()
        {
            base.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
