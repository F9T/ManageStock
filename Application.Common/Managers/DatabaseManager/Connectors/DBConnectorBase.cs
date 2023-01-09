using Application.Common.Managers.DatabaseManager.Conditions;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Application.Common.Managers.DatabaseManager
{
    internal abstract class DBConnectorBase
    {
        protected bool? m_IsLocked;

        protected DBConnectorBase()
        {
            m_IsLocked = null;
        }

        public abstract string ParameterChar { get; }

        public abstract string ParameterInsertChar { get; }

        public abstract EnumDBConnectorType ConnectorType { get; }


        protected string ColumnsInsertToString(ColumnsList _Columns)
        {
            StringBuilder str = new StringBuilder("(");
            for (var index = 0; index < _Columns.Count; index++)
            {
                ColumnResult column = _Columns[index];
                str.Append($"{column.ColumnName}");

                if (index + 1 < _Columns.Count)
                {
                    str.Append(", ");
                }
            }
            str.Append(")");

            if (ConnectorType == EnumDBConnectorType.SQLServer)
                str.Append(" output INSERTED.Id ");

            str.Append(" VALUES (");
            for (var index = 0; index < _Columns.Count; index++)
            {
                str.Append($"{ParameterInsertChar}{_Columns[index].ColumnName}");

                if (index + 1 < _Columns.Count)
                {
                    str.Append(", ");
                }
            }
            str.Append(")");

            return str.ToString();
        }

        protected string ColumnsUpdateToString(ColumnsList _Columns)
        {
            StringBuilder str = new StringBuilder();
            for (var index = 0; index < _Columns.Count; index++)
            {
                ColumnResult column = _Columns[index];
                str.Append($"{column.ColumnName} = {ParameterChar}{column.ColumnName}");

                if (index + 1 < _Columns.Count)
                {
                    str.Append(", ");
                }
            }

            return str.ToString();
        }

        protected string ConditionsToString(List<ConditionBase> _Conditions)
        {
            StringBuilder str = new StringBuilder();
            for (var index = 0; index < _Conditions.Count; index++)
            {
                ConditionBase conditionBase = _Conditions[index];
                str.Append(conditionBase);

                if (index + 1 < _Conditions.Count)
                {
                    str.Append(" ");
                }
            }

            return str.ToString();
        }

        protected List<DatabaseParameter> ColumnsToParameters(ColumnsList _Columns)
        {
            List<DatabaseParameter> parameters = new List<DatabaseParameter>();
            foreach (ColumnResult column in _Columns)
            {
                DatabaseParameter parameter = new DatabaseParameter(column.ColumnName, column.Type, column.ColumnName, column.Value);
                parameters.Add(parameter);
            }

            return parameters;
        }

        protected List<DatabaseParameter> ConditionsToParameters(List<ConditionBase> _Conditions)
        {
            List<DatabaseParameter> parameters = new List<DatabaseParameter>();
            foreach (ConditionBase conditionBase in _Conditions)
            {
                if (conditionBase is Condition condition)
                {
                    parameters.Add(condition.GetParameter());
                }
                else if (conditionBase is ConditionGroup group)
                {
                    parameters.AddRange(group.GetParameters().ToArray());
                }
            }

            return parameters;
        }


        public abstract bool Open(string _ConnectionString);

        public abstract bool Insert(string _TableName, ColumnsList _Columns, out int _Id);

        public abstract bool Update(string _TableName, ColumnsList _Columns, List<ConditionBase> _WhereCondition);

        public abstract bool Delete(string _TableName, List<ConditionBase> _WhereCondition);

        public abstract bool IsLocked();
        public abstract void SetLock(bool _IsLocked);

        public abstract List<RowResult> SelectQuery(string _Query);
        public abstract bool IsOpen();
        public abstract void Close();
    }
}
