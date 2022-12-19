using Application.Common.Logger;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Application.Common.Managers.DatabaseManagerBase
{
    public class DBManager
    {
        private static DBManager s_Instance;
        private SQLiteConnection m_DbConnection;
        private bool? m_IsLocked;

        private DBManager()
        {
            m_IsLocked = null;
        }

        public static DBManager InstanceOf => s_Instance ?? (s_Instance = new DBManager());

        private string ColumnsInsertToString(ColumnsList _Columns)
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

            str.Append(" VALUES (");
            for (var index = 0; index < _Columns.Count; index++)
            {
                str.Append("?");

                if (index + 1 < _Columns.Count)
                {
                    str.Append(", ");
                }
            }
            str.Append(")");

            return str.ToString();
        }

        private string ColumnsUpdateToString(ColumnsList _Columns)
        {
            StringBuilder str = new StringBuilder();
            for (var index = 0; index < _Columns.Count; index++)
            {
                ColumnResult column = _Columns[index];
                str.Append($"{column.ColumnName} = :{column.ColumnName}");

                if (index + 1 < _Columns.Count)
                {
                    str.Append(", ");
                }
            }

            return str.ToString();
        }

        private string ConditionsToString(List<ConditionBase> _Conditions)
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

        private List<SQLiteParameter> ColumnsToParameters(ColumnsList _Columns)
        {
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            foreach (ColumnResult column in _Columns)
            {
                SQLiteParameter parameter = new SQLiteParameter(column.Type.ToDbType())
                {
                    ParameterName = column.ColumnName,
                    Value = column.Value
                };

                parameters.Add(parameter);
            }

            return parameters;
        }

        private List<SQLiteParameter> ConditionsToParameters(List<ConditionBase> _Conditions)
        {
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
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

        public bool Open(string _ConnectionString)
        {
            if (m_DbConnection != null && IsOpen())
            {
                Close();
            }

            try
            {
                string databaseString = $"Data Source={_ConnectionString};Version=3;";
                m_DbConnection = new SQLiteConnection(databaseString);
                m_DbConnection.Open();

                return true;
            }
            catch (SQLiteException e)
            {
                m_DbConnection = null;
                ApplicationLogger.InstanceOf.Write(e.Message);
            }

            return false;
        }

        public bool Insert(string _TableName, ColumnsList _Columns, out int _Id)
        {
            _Id = -1;
            if (m_DbConnection == null)
            {
                return false;
            }

            if (m_DbConnection.State == ConnectionState.Open)
            {
                StringBuilder query = new StringBuilder($"INSERT INTO {_TableName} ");
                try
                {
                    if (_Columns.Count == 0)
                        return false;

                    query.Append(ColumnsInsertToString(_Columns));

                    query.Append(";");

                    using (SQLiteTransaction transaction = m_DbConnection.BeginTransaction())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(m_DbConnection))
                        {
                            command.CommandText = query.ToString();
                            command.Parameters.AddRange(ColumnsToParameters(_Columns).ToArray());

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }

                    _Id = (int)m_DbConnection.LastInsertRowId;

                    return true;
                }
                catch (SQLiteException e)
                {
                    ApplicationLogger.InstanceOf.Write($"Update({query}) {e.Message}");
                }
            }

            return false;
        }

        public bool Update(string _TableName, ColumnsList _Columns, List<ConditionBase> _WhereCondition)
        {
            if (m_DbConnection == null)
            {
                return false;
            }

            if (m_DbConnection.State == ConnectionState.Open)
            {
                StringBuilder query = new StringBuilder($"UPDATE {_TableName} SET ");
                try
                {
                    if (_Columns.Count == 0)
                        return false;

                    query.Append(ColumnsUpdateToString(_Columns));

                    if (_WhereCondition.Count > 0)
                    {
                        query.Append(" WHERE ");
                        query.Append(ConditionsToString(_WhereCondition));
                    }

                    query.Append(";");

                    using (SQLiteTransaction transaction = m_DbConnection.BeginTransaction())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(m_DbConnection))
                        {
                            command.CommandText = query.ToString();
                            command.Parameters.AddRange(ColumnsToParameters(_Columns).ToArray());
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToArray());

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }

                    return true;
                }
                catch (SQLiteException e)
                {
                    ApplicationLogger.InstanceOf.Write($"Update({query}) {e.Message}");
                }
            }

            return false;
        }

        public bool Delete(string _TableName, List<ConditionBase> _WhereCondition)
        {
            if (m_DbConnection == null)
            {
                return false;
            }

            if (m_DbConnection.State == ConnectionState.Open)
            {
                StringBuilder query = new StringBuilder($"DELETE FROM {_TableName}");

                try
                {
                    if (_WhereCondition.Count > 0)
                    {
                        query.Append(" WHERE ");
                        query.Append(ConditionsToString(_WhereCondition));
                    }

                    query.Append(";");

                    using (SQLiteTransaction transaction = m_DbConnection.BeginTransaction())
                    {
                        using (SQLiteCommand command = m_DbConnection.CreateCommand())
                        {
                            command.CommandText = query.ToString();
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToArray());

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }

                    return true;
                }
                catch (SQLiteException e)
                {
                    ApplicationLogger.InstanceOf.Write($"Delete({query}) {e.Message}");
                }
            }

            return false;
        }

        public bool IsLocked()
        {
            if (m_DbConnection == null)
            {
                return false;
            }

            if (!m_IsLocked.HasValue)
            {
                m_IsLocked = false;
                try
                {
                    if (m_DbConnection.State == ConnectionState.Open)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(m_DbConnection))
                        {
                            command.CommandText = "SELECT mutex FROM Lock;";
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int value = reader.GetInt32(0);
                                    m_IsLocked = value != 0;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (SQLiteException e)
                {
                    ApplicationLogger.InstanceOf.Write($"IsLocked : {e.Message}");
                }
            }

            return m_IsLocked.Value;
        }

        public void SetLock(bool _IsLocked)
        {
            if (m_DbConnection == null)
            {
                return;
            }

            if (m_DbConnection.State == ConnectionState.Open)
            {
                int value = _IsLocked ? 1 : 0;

                try
                {
                    using (SQLiteCommand command = new SQLiteCommand(m_DbConnection))
                    {
                        command.CommandText = "UPDATE Lock SET mutex=:mutex WHERE id=:id;";
                        command.Parameters.Add("mutex", DbType.Int32).Value = value;
                        command.Parameters.Add("id", DbType.Int32).Value = 1;

                        command.ExecuteNonQuery();
                    }

                    m_IsLocked = _IsLocked;
                }
                catch (SQLiteException e)
                {
                    ApplicationLogger.InstanceOf.Write($"SetLock : {e.Message}");
                }
            }
        }

        public List<RowResult> SelectQuery(string _Query)
        {
            if (m_DbConnection == null)
                return new List<RowResult>();

            List<RowResult> results = new List<RowResult>();
            if (m_DbConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (SQLiteCommand command = new SQLiteCommand(m_DbConnection))
                    {
                        command.CommandText = _Query;

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RowResult row = new RowResult();
                                var values = reader.GetValues();

                                for (int i = 0; i < values.Count; i++)
                                {
                                    var column = values.GetKey(i);
                                    var value = values.Get(i);
                                    var type = reader.GetFieldType(i);

                                    row.ColumnsList.Add(new ColumnResult(column, value, type));
                                }

                                results.Add(row);
                            }
                        }

                        return results;
                    }
                }
                catch (SQLiteException)
                {
                    return results;
                }
            }

            return results;
        }

        public bool IsOpen()
        {
            return m_DbConnection != null && m_DbConnection.State == ConnectionState.Open;
        }

        public void Close()
        {
            if (m_DbConnection != null)
            {
                try
                {
                    m_DbConnection.Close();
                }
                catch (SQLiteException)
                {

                }
            }
        }
    }
}
