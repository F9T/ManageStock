using System;
using System.IO;

namespace Application.Common.DatabaseInformation
{
    public class DatabaseCreator
    {
        public static bool CreateDatabase(DatabaseInfo _DatabaseInfo)
        {
            if (_DatabaseInfo == null) return false;

            switch (_DatabaseInfo.Type)
            {
                case Managers.DatabaseManager.EnumDBConnectorType.SQLite:
                    return CreateSQLiteDatabase(_DatabaseInfo);
                case Managers.DatabaseManager.EnumDBConnectorType.SQLServer:
                    break;
                case Managers.DatabaseManager.EnumDBConnectorType.MySQL:
                    break;
            }

            return false;
        }

        private static bool CreateSQLiteDatabase(DatabaseInfo _DatabaseInfo)
        {
            string emptyDatabasePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Databases", "empty.db");
            if(!File.Exists(emptyDatabasePath))
            {
                return false;
            }
            try
            {
                File.Copy(emptyDatabasePath, _DatabaseInfo.ConnectionString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
