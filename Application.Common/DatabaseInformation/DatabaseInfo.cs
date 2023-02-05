using Application.Common.Managers.DatabaseManager;

namespace Application.Common.DatabaseInformation
{
    public class DatabaseInfo
    {
        public DatabaseInfo()
        {

        }

        public DatabaseInfo(string _Name, EnumDBConnectorType _Type, string _ConnectionString)
        {
            Name = _Name;
            Type = _Type;
            ConnectionString = _ConnectionString;
        }

        public string Name { get; set; }

        public EnumDBConnectorType Type { get; set; }

        public string ConnectionString { get; set; }
    }
}
