using System;
using System.IO;

namespace Application.Common.Logger
{
    public class ApplicationLogger
    {
        private static ApplicationLogger s_Instance;
        private string m_Path;

        private ApplicationLogger()
        {
        }

        public static ApplicationLogger InstanceOf => s_Instance ?? (s_Instance = new ApplicationLogger());

        public void SetLoggerPath(string _Path)
        {
            m_Path = _Path;
        }

        public void Write(string _Text)
        {
            if (string.IsNullOrEmpty(m_Path))
                return;

            string log = $"{DateTime.Now:dd.MM.yyyy - hh:mm:ss}  {_Text}";

            File.AppendAllText(m_Path, log);
        }
    }
}
