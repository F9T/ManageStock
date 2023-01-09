using Application.Common.Managers.DatabaseManager;
using Application.Common.Managers.DatabaseManager.Connectors;
using Application.Common.Managers.DatabaseManagerBase.Conditions;
using System;
using System.Collections.Generic;

namespace Application.Common.Managers.DatabaseManagerBase
{
    public class DBManager
    {
        private static DBManager s_Instance;
        private DBConnectorBase m_Connector;

        private DBManager()
        {
        }

        public static DBManager InstanceOf => s_Instance ?? (s_Instance = new DBManager());

        public bool Open(EnumDBConnectorType _ConnectorType, string _ConnectionString)
        {
            if (m_Connector != null && m_Connector.IsOpen())
            {
                Close();
            }

            switch (_ConnectorType)
            {
                case EnumDBConnectorType.SQLite:
                    m_Connector = new SQLiteConnector();
                    break;
                case EnumDBConnectorType.SQLServer:
                    m_Connector = new SQLServerConnector();
                    break;
                case EnumDBConnectorType.MySQL:
                    m_Connector = new MySQLConnector();
                    break;
            }

            return m_Connector.Open(_ConnectionString);
        }

        public bool Insert(string _TableName, ColumnsList _Columns, out int _Id)
        {
            _Id = -1;
            if (m_Connector == null)
            {
                return false;
            }

            return m_Connector.Insert(_TableName, _Columns, out _Id);
        }

        public bool Update(string _TableName, ColumnsList _Columns, List<ConditionBase> _WhereCondition)
        {
            if (m_Connector == null)
            {
                return false;
            }

            return m_Connector.Update(_TableName, _Columns, _WhereCondition);
        }

        public bool Delete(string _TableName, List<ConditionBase> _WhereCondition)
        {
            if (m_Connector == null)
            {
                return false;
            }

            return m_Connector.Delete(_TableName, _WhereCondition);
        }

        public bool IsLocked()
        {
            if (m_Connector == null)
            {
                return false;
            }

            return m_Connector.IsLocked();
        }

        public void SetLock(bool _IsLocked)
        {
            if (m_Connector == null)
            {
                return;
            }

            m_Connector.SetLock(_IsLocked);
        }

        public List<RowResult> SelectQuery(string _Query)
        {
            if (m_Connector == null)
                return new List<RowResult>();

            return m_Connector.SelectQuery(_Query);
        }

        public bool IsOpen()
        {
            return m_Connector != null && m_Connector.IsOpen();
        }

        public void Close()
        {
            if (m_Connector != null)
            {
                try
                {
                    m_Connector.Close();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
