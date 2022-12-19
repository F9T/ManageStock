using Application.Common.Logger;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Application.Common.Settings
{
    public class SettingsSerializer<T> where T : SettingsBase
    {
        private XmlSerializer m_Serializer;

        public SettingsSerializer()
        {
            m_Serializer = new XmlSerializer(typeof(SettingsBase), new[] { typeof(T) });
        }

        public bool Load(string _Path, out T _Settings)
        {
            try
            {
                T settings = default;

                using (StreamReader reader = new StreamReader(_Path))
                {
                    settings = (T)m_Serializer.Deserialize(reader);
                }

                _Settings = settings;
                return true;
            }
            catch (Exception e)
            {
                _Settings = default;

                ApplicationLogger.InstanceOf.Write(e.Message);

                return false;
            }
        }

        public bool Save(string _Path, T _Settings)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_Path))
                {
                    m_Serializer.Serialize(writer, _Settings);
                }

                return true;
            }
            catch (Exception e)
            {
                ApplicationLogger.InstanceOf.Write(e.Message);
                return false;
            }
        }
    }
}
