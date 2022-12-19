using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Application.Common.PathConfiguration
{
    public class PathManager
    {
        private static PathManager s_Instance;
        private Dictionary<EnumConfigurationPath, string> m_Paths;

        private PathManager()
        {
            m_Paths = new Dictionary<EnumConfigurationPath, string>
            {
                {EnumConfigurationPath.Settings, @"%APPDATA%\ManageStock\settings.xml"},
                {EnumConfigurationPath.Database, "ManageStock.db"},
                {EnumConfigurationPath.Logs, @"%APPDATA%\ManageStock\app_trace.log"}
            };
        }

        public static PathManager InstanceOf => s_Instance ?? (s_Instance = new PathManager());

        public string this[EnumConfigurationPath _Configuration]
        {
            get => Environment.ExpandEnvironmentVariables(m_Paths[_Configuration]);
            set => m_Paths[_Configuration] = value;
        }
    }
}
