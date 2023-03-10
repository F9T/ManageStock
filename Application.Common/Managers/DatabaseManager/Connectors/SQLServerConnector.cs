using Application.Common.Logger;
using Application.Common.Managers.DatabaseManager.Conditions;
using Application.Common.Managers.DatabaseManagerBase;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Application.Common.Managers.DatabaseManager.Connectors
{
    internal sealed class SQLServerConnector : DBConnectorBase
    {
        private SqlConnection m_DbConnection;

        public override string ParameterChar => "@";

        public override string ParameterInsertChar => "@";
        public override EnumDBConnectorType ConnectorType => EnumDBConnectorType.SQLServer;
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

                    using (SqlCommand command = new SqlCommand(query.ToString(), m_DbConnection))
                    {
                        command.Parameters.AddRange(ColumnsToParameters(_Columns).ToSQLServerParameter().ToList().ToArray());

                        _Id = (int)command.ExecuteScalar();
                    }

                    return true;
                }
                catch (SqlException e)
                {
                    ApplicationLogger.InstanceOf.Write($"Update({query}) {e.Message}");
                }
            }

            return false;
        }
        public override bool Open(string _ConnectionString)
        {
            if (m_DbConnection != null && IsOpen())
            {
                Close();
            }

            try
            {
                string databaseString = $@"Data Source=DESKTOP-UF0LMPR\TLSQLSERVER;Initial Catalog=master;User ID=sa;Password=poire";
                m_DbConnection = new SqlConnection(databaseString);
                m_DbConnection.Open();

                return true;
            }
            catch (SqlException e)
            {
                m_DbConnection = null;
                ApplicationLogger.InstanceOf.Write(e.Message);
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

                    using (SqlCommand command = new SqlCommand(query.ToString(), m_DbConnection))
                    {
                        using (SqlTransaction transaction = m_DbConnection.BeginTransaction())
                        {
                            command.Transaction = transaction;

                            command.Parameters.AddRange(ColumnsToParameters(_Columns).ToSQLServerParameter().ToList().ToArray());
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToSQLServerParameter().ToList().ToArray());

                            command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }

                    return true;
                }
                catch (SqlException e)
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

                    using (SqlCommand command = m_DbConnection.CreateCommand())
                    {
                        using (SqlTransaction transaction = m_DbConnection.BeginTransaction())
                        {
                            command.Transaction = transaction;

                            command.CommandText = query.ToString();
                            command.Parameters.AddRange(ConditionsToParameters(_WhereCondition).ToSQLServerParameter().ToList().ToArray());

                            command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }

                    return true;
                }
                catch (SqlException e)
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
                        using (SqlCommand command = new SqlCommand("SELECT mutex FROM Lock;", m_DbConnection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
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
                catch (SqlException e)
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
                    using (SqlCommand command = new SqlCommand("UPDATE Lock SET mutex=@mutex WHERE id=@id;", m_DbConnection))
                    {
                        command.Parameters.AddWithValue("mutex", value);
                        command.Parameters.AddWithValue("id", 1);

                        command.ExecuteNonQuery();
                    }

                    m_IsLocked = _IsLocked;
                }
                catch (SqlException e)
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
                    using (SqlCommand command = new SqlCommand(_Query, m_DbConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RowResult row = new RowResult();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var column = reader.GetName(i);
                                    var value = reader.GetValue(i);
                                    var type = reader.GetFieldType(i);

                                    row.ColumnsList.Add(new ColumnResult(column, value, type));
                                }

                                results.Add(row);
                            }
                        }

                        return results;
                    }
                }
                catch (SqlException)
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
                catch (SqlException)
                {

                }
            }
        }
    }
}
