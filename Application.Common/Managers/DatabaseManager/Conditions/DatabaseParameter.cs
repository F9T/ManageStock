using System;
using System.Data;

namespace Application.Common.Managers.DatabaseManager.Conditions
{
    internal class DatabaseParameter
    {
        public DatabaseParameter(string columnName, Type dbType, string parameterName, object value)
        {
            ColumnName = columnName;
            DbType = dbType;
            ParameterName = parameterName;
            Value = value;
        }

        public string ColumnName { get; }

        public Type DbType { get; }

        public string ParameterName { get; }

        public object Value { get; }
    }
}