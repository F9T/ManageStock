using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Application.Common.Managers.DatabaseManagerBase
{
    internal static class TypeHelper
    {
        static Dictionary<Type, DbType> s_SQLiteType = new Dictionary<Type, DbType>();
        static Dictionary<Type, SqlDbType> s_SQLServerType = new Dictionary<Type, SqlDbType>();
        static Dictionary<Type, MySqlDbType> s_MySQLType = new Dictionary<Type, MySqlDbType>();
        static TypeHelper()
        {
            s_SQLiteType[typeof(byte)] = DbType.Byte;
            s_SQLiteType[typeof(sbyte)] = DbType.SByte;
            s_SQLiteType[typeof(short)] = DbType.Int16;
            s_SQLiteType[typeof(ushort)] = DbType.UInt16;
            s_SQLiteType[typeof(int)] = DbType.Int32;
            s_SQLiteType[typeof(uint)] = DbType.UInt32;
            s_SQLiteType[typeof(long)] = DbType.Int64;
            s_SQLiteType[typeof(ulong)] = DbType.UInt64;
            s_SQLiteType[typeof(float)] = DbType.Single;
            s_SQLiteType[typeof(double)] = DbType.Double;
            s_SQLiteType[typeof(decimal)] = DbType.Decimal;
            s_SQLiteType[typeof(bool)] = DbType.Boolean;
            s_SQLiteType[typeof(string)] = DbType.String;
            s_SQLiteType[typeof(char)] = DbType.StringFixedLength;
            s_SQLiteType[typeof(Guid)] = DbType.Guid;
            s_SQLiteType[typeof(DateTime)] = DbType.DateTime;
            s_SQLiteType[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            s_SQLiteType[typeof(byte[])] = DbType.Binary;
            s_SQLiteType[typeof(byte?)] = DbType.Byte;
            s_SQLiteType[typeof(sbyte?)] = DbType.SByte;
            s_SQLiteType[typeof(short?)] = DbType.Int16;
            s_SQLiteType[typeof(ushort?)] = DbType.UInt16;
            s_SQLiteType[typeof(int?)] = DbType.Int32;
            s_SQLiteType[typeof(uint?)] = DbType.UInt32;
            s_SQLiteType[typeof(long?)] = DbType.Int64;
            s_SQLiteType[typeof(ulong?)] = DbType.UInt64;
            s_SQLiteType[typeof(float?)] = DbType.Single;
            s_SQLiteType[typeof(double?)] = DbType.Double;
            s_SQLiteType[typeof(decimal?)] = DbType.Decimal;
            s_SQLiteType[typeof(bool?)] = DbType.Boolean;
            s_SQLiteType[typeof(char?)] = DbType.StringFixedLength;
            s_SQLiteType[typeof(Guid?)] = DbType.Guid;
            s_SQLiteType[typeof(DateTime?)] = DbType.DateTime;
            s_SQLiteType[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;


            s_SQLServerType[typeof(byte)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(sbyte)] = SqlDbType.TinyInt;
            s_SQLServerType[typeof(short)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(ushort)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(int)] = SqlDbType.Int;
            s_SQLServerType[typeof(uint)] = SqlDbType.Int;
            s_SQLServerType[typeof(long)] = SqlDbType.BigInt;
            s_SQLServerType[typeof(ulong)] = SqlDbType.BigInt;
            s_SQLServerType[typeof(float)] = SqlDbType.Real;
            s_SQLServerType[typeof(double)] = SqlDbType.Real;
            s_SQLServerType[typeof(decimal)] = SqlDbType.Decimal;
            s_SQLServerType[typeof(bool)] = SqlDbType.Bit;
            s_SQLServerType[typeof(string)] = SqlDbType.Text;
            s_SQLServerType[typeof(char)] = SqlDbType.Char;
            s_SQLServerType[typeof(Guid)] = SqlDbType.VarChar;
            s_SQLServerType[typeof(DateTime)] = SqlDbType.DateTime;
            s_SQLServerType[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            s_SQLServerType[typeof(byte[])] = SqlDbType.Binary;
            s_SQLServerType[typeof(byte?)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(sbyte?)] = SqlDbType.TinyInt;
            s_SQLServerType[typeof(short?)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(ushort?)] = SqlDbType.SmallInt;
            s_SQLServerType[typeof(int?)] = SqlDbType.Int;
            s_SQLServerType[typeof(uint?)] = SqlDbType.Int;
            s_SQLServerType[typeof(long?)] = SqlDbType.BigInt;
            s_SQLServerType[typeof(ulong?)] = SqlDbType.BigInt;
            s_SQLServerType[typeof(float?)] = SqlDbType.Real;
            s_SQLServerType[typeof(double?)] = SqlDbType.Real;
            s_SQLServerType[typeof(decimal?)] = SqlDbType.Decimal;
            s_SQLServerType[typeof(bool?)] = SqlDbType.Bit;
            s_SQLServerType[typeof(char?)] = SqlDbType.Char;
            s_SQLServerType[typeof(Guid?)] = SqlDbType.VarChar;
            s_SQLServerType[typeof(DateTime?)] = SqlDbType.DateTime;
            s_SQLServerType[typeof(DateTimeOffset?)] = SqlDbType.DateTimeOffset;


            s_MySQLType[typeof(byte)] = MySqlDbType.Byte;
            s_MySQLType[typeof(sbyte)] = MySqlDbType.UByte;
            s_MySQLType[typeof(short)] = MySqlDbType.Int16;
            s_MySQLType[typeof(ushort)] = MySqlDbType.UInt16;
            s_MySQLType[typeof(int)] = MySqlDbType.Int32;
            s_MySQLType[typeof(uint)] = MySqlDbType.UInt32;
            s_MySQLType[typeof(long)] = MySqlDbType.Int64;
            s_MySQLType[typeof(ulong)] = MySqlDbType.UInt64;
            s_MySQLType[typeof(float)] = MySqlDbType.Float;
            s_MySQLType[typeof(double)] = MySqlDbType.Double;
            s_MySQLType[typeof(decimal)] = MySqlDbType.Decimal;
            s_MySQLType[typeof(bool)] = MySqlDbType.Bit;
            s_MySQLType[typeof(string)] = MySqlDbType.String;
            s_MySQLType[typeof(char)] = MySqlDbType.VarChar;
            s_MySQLType[typeof(Guid)] = MySqlDbType.Guid;
            s_MySQLType[typeof(DateTime)] = MySqlDbType.DateTime;
            s_MySQLType[typeof(byte[])] = MySqlDbType.Binary;
            s_MySQLType[typeof(byte?)] = MySqlDbType.Byte;
            s_MySQLType[typeof(sbyte?)] = MySqlDbType.UByte;
            s_MySQLType[typeof(short?)] = MySqlDbType.Int16;
            s_MySQLType[typeof(ushort?)] = MySqlDbType.UInt16;
            s_MySQLType[typeof(int?)] = MySqlDbType.Int32;
            s_MySQLType[typeof(uint?)] = MySqlDbType.UInt32;
            s_MySQLType[typeof(long?)] = MySqlDbType.Int64;
            s_MySQLType[typeof(ulong?)] = MySqlDbType.UInt64;
            s_MySQLType[typeof(float?)] = MySqlDbType.Float;
            s_MySQLType[typeof(double?)] = MySqlDbType.Double;
            s_MySQLType[typeof(decimal?)] = MySqlDbType.Decimal;
            s_MySQLType[typeof(bool?)] = MySqlDbType.Bit;
            s_MySQLType[typeof(char?)] = MySqlDbType.VarChar;
            s_MySQLType[typeof(Guid?)] = MySqlDbType.Guid;
            s_MySQLType[typeof(DateTime?)] = MySqlDbType.DateTime;
        }

        public static DbType ToSQLiteDbType(this Type _Type)
        {
            if (!s_SQLiteType.ContainsKey(_Type))
                return DbType.String;

            return s_SQLiteType[_Type];
        }

        public static SqlDbType ToSQLServerDbType(this Type _Type)
        {
            if (!s_SQLiteType.ContainsKey(_Type))
                return SqlDbType.Text;

            return s_SQLServerType[_Type];
        }
        public static MySqlDbType ToMySQLDbType(this Type _Type)
        {
            if (!s_SQLiteType.ContainsKey(_Type))
                return MySqlDbType.String;

            return s_MySQLType[_Type];
        }

        public static Type ToType(this DbType _DbType)
        {
            if (!s_SQLiteType.ContainsValue(_DbType))
                return typeof(string);

            foreach (KeyValuePair<Type, DbType> keyValuePair in s_SQLiteType)
            {
                if (keyValuePair.Value == _DbType)
                    return keyValuePair.Key;
            }

            return typeof(string);
        }
    }
}
