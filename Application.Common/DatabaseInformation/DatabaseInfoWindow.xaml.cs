using Application.Common.Commands;
using Application.Common.Managers.DatabaseManager;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace Application.Common.DatabaseInformation
{
    /// <summary>
    /// Logique d'interaction pour DatabaseInfoWindow.xaml
    /// </summary>
    public partial class DatabaseInfoWindow : CustomWindow, INotifyPropertyChanged
    {
        private string m_ConnectionString;
        private string m_DatabaseName;
        private EnumDBConnectorType m_ConnectorType;

        public DatabaseInfoWindow()
        {
            InitializeComponent();
            CreateCommand = new RelayCommand(_ => Create(), _ => VerifyData());

            ConnectorType = EnumDBConnectorType.SQLite;

            DataContext = this;
        }

        public ICommand CreateCommand { get; set; }

        public EnumDBConnectorType ConnectorType
        {
            get => m_ConnectorType;
            set
            {
                m_ConnectorType = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionString
        {
            get => m_ConnectionString;
            set
            {
                m_ConnectionString = value;
                OnPropertyChanged();
            }
        }

        public string DatabaseName
        {
            get => m_DatabaseName;
            set
            {
                m_DatabaseName = value;
                OnPropertyChanged();
            }
        }

        private void BrowsePathButtonOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ConnectionString = dialog.SelectedPath;
            }
        }

        private void CancelButtonOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Create()
        {
            if(ConnectorType == EnumDBConnectorType.SQLite)
            {
                ConnectionString = Path.Combine(ConnectionString, $"{Path.GetFileNameWithoutExtension(DatabaseName)}.db");
            }

            DialogResult = true;
        }

        private bool VerifyData()
        {
            return !string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(DatabaseName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
