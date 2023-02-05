using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Application.Common.DatabaseInformation
{
    public class DatabaseInfoSerializer
    {
        private static XmlSerializer s_Serializer = new XmlSerializer(typeof(List<DatabaseInfo>));

        public static bool Load(string _Path, out List<DatabaseInfo> _DatabasesInfo)
        {
            try
            {
                using (StreamReader reader = new StreamReader(_Path))
                {
                    _DatabasesInfo = (List<DatabaseInfo>)s_Serializer.Deserialize(reader);
                }
                return true;
            }
            catch (Exception)
            {
                _DatabasesInfo = new List<DatabaseInfo>();
                return false;
            }
        }

        public static bool Save(string _Path, List<DatabaseInfo> _DatabasesInfo)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_Path))
                {
                    s_Serializer.Serialize(writer, _DatabasesInfo);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
