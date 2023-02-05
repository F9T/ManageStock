namespace Application.Common.AppManager
{
    public class ApplicationManager
    {
        private static ApplicationManager s_ApplicationManager;

        private ApplicationManager()
        {
            ApplicationContext = EnumApplicationContext.None;
        }

        public static ApplicationManager InstanceOf => s_ApplicationManager ?? (s_ApplicationManager = new ApplicationManager());

        public EnumApplicationContext ApplicationContext { get; set; }
    }
}
