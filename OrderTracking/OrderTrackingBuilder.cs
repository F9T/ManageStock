using Application.Common;
using Application.Common.DatabaseInformation;
using OrderTracking.Views;
using System.Windows.Controls;

namespace OrderTracking
{
    public class OrderTrackingBuilder : AppBuilderBase
    {
        private ArticlesMainView m_MainView;

        public override UserControl View => m_MainView;

        public override void Launch(DatabaseInfo _DatabaseInfo, object[] _Args = null)
        {
            base.Launch(_DatabaseInfo, _Args);

            m_MainView = new ArticlesMainView();
            m_MainView.MainViewModel.Initialize(CustomNotificationsManager);
        }

        public override void Shutdown(int _ExitCode)
        {
            base.Shutdown(_ExitCode);
        }

        public override void Dispose()
        {

        }
    }
}
