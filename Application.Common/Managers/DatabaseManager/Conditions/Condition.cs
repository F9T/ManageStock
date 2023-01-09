using Application.Common.Managers.DatabaseManager.Conditions;

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

        public DatabaseParameter GetParameter()
        {
            return new DatabaseParameter(ColumnName, Value.GetType(), ColumnName, Value);
        }

        public override string ToString()
        {
            return $"{ColumnName} {Operator} @{ColumnName}";
        }
    }
}
