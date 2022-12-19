using System.Data.SQLite;

namespace Application.Common.Managers.DatabaseManagerBase.Conditions
{
    internal class Condition : ConditionBase
    {
        public Condition(string columnName, string @operator, object value)
        {
            ColumnName = columnName;
            Operator = @operator;
            Value = value;
        }

        public string ColumnName { get; }

        public string Operator { get; }

        public object Value { get; }

        public SQLiteParameter GetParameter()
        {
            SQLiteParameter parameter = new SQLiteParameter(Value.GetType().ToDbType())
            {
                ParameterName = ColumnName,
                Value = Value
            };
            return parameter;
        }

        public override string ToString()
        {
            return $"{ColumnName} {Operator} :{ColumnName}";
        }
    }
}
