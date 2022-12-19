using System;
using System.Collections.Generic;
using System.Data;

namespace Application.Common.Managers.DatabaseManagerBase
{
    internal static class TypeHelper
    {
        static Dictionary<Type, DbType> s_TypeMap = new Dictionary<Type, DbType>();
        static TypeHelper()
        {
            s_TypeMap[typeof(byte)] = DbType.Byte;
            s_TypeMap[typeof(sbyte)] = DbType.SByte;
            s_TypeMap[typeof(short)] = DbType.Int16;
            s_TypeMap[typeof(ushort)] = DbType.UInt16;
            s_TypeMap[typeof(int)] = DbType.Int32;
            s_TypeMap[typeof(uint)] = DbType.UInt32;
            s_TypeMap[typeof(long)] = DbType.Int64;
            s_TypeMap[typeof(ulong)] = DbType.UInt64;
            s_TypeMap[typeof(float)] = DbType.Single;
            s_TypeMap[typeof(double)] = DbType.Double;
            s_TypeMap[typeof(decimal)] = DbType.Decimal;
            s_TypeMap[typeof(bool)] = DbType.Boolean;
            s_TypeMap[typeof(string)] = DbType.String;
            s_TypeMap[typeof(char)] = DbType.StringFixedLength;
            s_TypeMap[typeof(Guid)] = DbType.Guid;
            s_TypeMap[typeof(DateTime)] = DbType.DateTime;
            s_TypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            s_TypeMap[typeof(byte[])] = DbType.Binary;
            s_TypeMap[typeof(byte?)] = DbType.Byte;
            s_TypeMap[typeof(sbyte?)] = DbType.SByte;
            s_TypeMap[typeof(short?)] = DbType.Int16;
            s_TypeMap[typeof(ushort?)] = DbType.UInt16;
            s_TypeMap[typeof(int?)] = DbType.Int32;
            s_TypeMap[typeof(uint?)] = DbType.UInt32;
            s_TypeMap[typeof(long?)] = DbType.Int64;
            s_TypeMap[typeof(ulong?)] = DbType.UInt64;
            s_TypeMap[typeof(float?)] = DbType.Single;
            s_TypeMap[typeof(double?)] = DbType.Double;
            s_TypeMap[typeof(decimal?)] = DbType.Decimal;
            s_TypeMap[typeof(bool?)] = DbType.Boolean;
            s_TypeMap[typeof(char?)] = DbType.StringFixedLength;
            s_TypeMap[typeof(Guid?)] = DbType.Guid;
            s_TypeMap[typeof(DateTime?)] = DbType.DateTime;
            s_TypeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
        }

        public static DbType ToDbType(this Type _Type)
        {
            if (!s_TypeMap.ContainsKey(_Type))
                return DbType.String;

            return s_TypeMap[_Type];
        }

        public static Type ToType(this DbType _DbType)
        {
            if (!s_TypeMap.ContainsValue(_DbType))
                return typeof(string);

            foreach (KeyValuePair<Type, DbType> keyValuePair in s_TypeMap)
            {
                if (keyValuePair.Value == _DbType)
                    return keyValuePair.Key;
            }

            return typeof(string);
        }
    }
}
