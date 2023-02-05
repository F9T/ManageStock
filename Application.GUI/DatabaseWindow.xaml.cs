using Application.Common;
using Application.Common.DatabaseInformation;
using MaterialDesignExtensions.Controls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Windows;

namespace Application.GUI
{
    /// <summary>
    /// Logique d'interaction pour DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : CustomWindow
    {
        private bool m_ManuallyClosed;

        public DatabaseWindow(List<DatabaseInfo> _Databases)
        {
            InitializeComponent();

            Databases = new ObservableCollection<DatabaseInfo>(_Databases);
            m_ManuallyClosed = false;

            DataContext = this;
        }
        public ObservableCollection<DatabaseInfo> Databases { get; set; }

        public DatabaseInfo SelectedDatabaseInfo { get; private set; }

        private void OpenDatabaseButtonOnClick(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                var database = element.Tag as DatabaseInfo;
                if (database != null)
                {
                    SelectedDatabaseInfo = database;
                    m_ManuallyClosed = true;
                    DialogResult = true;
                }
            }
        }

        private void AddExistingDatabaseButtonOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            m_ManuallyClosed = true;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Databases Files|*.db"
            };

            if(dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                string name = Path.GetFileNameWithoutExtension(path);

                DatabaseInfo database = new DatabaseInfo(name, Common.Managers.DatabaseManager.EnumDBConnectorType.SQLite, path);
                Databases.Add(database);
                SelectedDatabaseInfo = database;
            }
        }

        private void ExitButtonOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            m_ManuallyClosed = true;
            System.Windows.Application.Current.Shutdown(0);
        }

        private void NewDatabaseButtonOnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DatabaseInfoWindow window = new DatabaseInfoWindow
            {
                Owner = this
            };
            if (window.ShowDialog() == true)
            {
                DatabaseInfo database = new DatabaseInfo(window.DatabaseName, window.ConnectorType, window.ConnectionString);

                if (DatabaseCreator.CreateDatabase(database))
                {
                    Databases.Add(database);
                    SelectedDatabaseInfo = database;
                }
                else
                {
                    MessageBox.Show("Une erreur est survenue à la création du stock. Veuillez ressayer.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CustomWindowClosed(object sender, System.EventArgs e)
        {
            if (m_ManuallyClosed)
            {
                return;
            }

            System.Windows.Application.Current.Shutdown(0);
        }
    }
}
