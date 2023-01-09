using Application.Common.Logger;
using Application.Common.Managers.DatabaseManager.Conditions;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Application.Common.Managers.DatabaseManager.Connectors
{
    internal class SQLiteConnector : DBConnectorBase
    {
        private SQLiteConnection m_DbConnection;

        public override string ParameterChar => ":";

        public override string ParameterInsertChar => "?";
        public override EnumDBConnectorType ConnectorType => EnumDBConnectorType.SQLite;

        public override bool Open(string _ConnectionString)
        {
            if (m_DbConnection != null && IsOpen())
            {
                Close();
            }

            try
            {
                string databaseString = $"Data Source={_ConnectionString};Version=3;journal mode=Off";
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

        public override bool Insert(string _TableName, ColumnsList _Columns, out int _Id)
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
                            command.Parameters.AddRange(ColumnsToParameters(_Columns).ToSQLiteParameter().ToList().ToArray());

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

        public override bool Update(string _TableName, ColumnsList _Columns, List<ConditionBase> _WhereCondition)
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
                            command.Parameters.AddRange(ColumnsToParameters(_Columns).ToSQLiteParameter().ToList().ToArray());
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToSQLiteParameter().ToList().ToArray());

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

        public override bool Delete(string _TableName, List<ConditionBase> _WhereCondition)
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
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToSQLiteParameter().ToList().ToArray());

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

        public override bool IsLocked()
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

        public override void SetLock(bool _IsLocked)
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

        public override List<RowResult> SelectQuery(string _Query)
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

        public override bool IsOpen()
        {
            return m_DbConnection != null && m_DbConnection.State == ConnectionState.Open;
        }

        public override void Close()
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
