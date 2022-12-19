using Application.Common;
using OrderTracking.Views;
using System.Windows.Controls;

namespace OrderTracking
{
    public class OrderTrackingBuilder : AppBuilderBase
    {
        private ArticlesMainView m_MainView;

        public override UserControl View => m_MainView;

        public override void Launch(object[] _Args = null)
        {
            base.Launch(_Args);

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
