using System;

namespace Application.Common.Managers.DatabaseManagerBase
{
    public struct ColumnResult
    {
        public ColumnResult(string columnName, object value, Type type)
        {
            ColumnName = columnName;
            Value = value;
            Type = type;
        }

        public string ColumnName { get; }

        public object Value { get; }

        public Type Type { get; }
    }
}
