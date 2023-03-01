using System;
using System.IO;

namespace Application.Common.DatabaseInformation
{
    public class DatabaseCreator
    {
        public static bool IsDatabaseExists(DatabaseInfo _DatabaseInfo)
        {
            if (_DatabaseInfo == null) return false;

            switch (_DatabaseInfo.Type)
            {
                case Managers.DatabaseManager.EnumDBConnectorType.SQLite:
                    return IsDatabaseSQLiteExists(_DatabaseInfo);
                case Managers.DatabaseManager.EnumDBConnectorType.SQLServer:
                    break;
                case Managers.DatabaseManager.EnumDBConnectorType.MySQL:
                    break;
            }


            return false;
        }
        private static bool IsDatabaseSQLiteExists(DatabaseInfo _DatabaseInfo)
        {
            var parent = Directory.GetParent(_DatabaseInfo.ConnectionString);


            return File.Exists(_DatabaseInfo.ConnectionString);
        }

        public static bool DeleteDatabase(DatabaseInfo _DatabaseInfo)
        {
            if (_DatabaseInfo == null) return false;

            switch (_DatabaseInfo.Type)
            {
                case Managers.DatabaseManager.EnumDBConnectorType.SQLite:
                    return DeleteSQLiteDatabase(_DatabaseInfo);
                case Managers.DatabaseManager.EnumDBConnectorType.SQLServer:
                    break;
                case Managers.DatabaseManager.EnumDBConnectorType.MySQL:
                    break;
            }


            return false;
        }

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

        private static bool DeleteSQLiteDatabase(DatabaseInfo _DatabaseInfo)
        {
            try
            {
                File.Delete(_DatabaseInfo.ConnectionString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CreateSQLiteDatabase(DatabaseInfo _DatabaseInfo)
        {
            string emptyDatabasePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Databases", "empty.db");
            if (!File.Exists(emptyDatabasePath))
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
