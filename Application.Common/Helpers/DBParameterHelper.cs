using Application.Common.Managers.DatabaseManagerBase;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Application.Common.Managers.DatabaseManager.Conditions
{
    internal static class DBParameterHelper
    {
        public static IEnumerable<SqlParameter> ToSQLServerParameter(this IEnumerable<DatabaseParameter> _Parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            foreach (var parameter in _Parameters)
            {
                sqlParameters.Add(new SqlParameter(parameter.ColumnName, parameter.DbType.ToSQLServerDbType())
                {
                    ParameterName = parameter.ParameterName,
                    Value = parameter.Value
                });
            }

            return sqlParameters;
        }
        public static IEnumerable<SQLiteParameter> ToSQLiteParameter(this IEnumerable<DatabaseParameter> _Parameters)
        {
            List<SQLiteParameter> sqlParameters = new List<SQLiteParameter>();

            foreach (var parameter in _Parameters)
            {
                sqlParameters.Add(new SQLiteParameter(parameter.ColumnName, parameter.DbType.ToSQLiteDbType())
                {
                    ParameterName = parameter.ParameterName,
                    Value = parameter.Value
                });
            }

            return sqlParameters;
        }

        public static IEnumerable<MySqlParameter> ToMySQLParameter(this IEnumerable<DatabaseParameter> _Parameters)
        {
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            foreach (var parameter in _Parameters)
            {
                sqlParameters.Add(new MySqlParameter(parameter.ColumnName, parameter.DbType.ToMySQLDbType())
                {
                    ParameterName = parameter.ParameterName,
                    Value = parameter.Value
                });
            }

            return sqlParameters;
        }
    }
}
