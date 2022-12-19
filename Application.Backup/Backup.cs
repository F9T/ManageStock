using System;
using System.IO;

namespace Application.Backup
{
    internal class Backup
    {
        private string m_FileNameWithoutExt;

        public string FileName { get; set; }

        public string FileNameWithoutExt
        {
            get
            {
                if (string.IsNullOrEmpty(m_FileNameWithoutExt) && !string.IsNullOrEmpty(FileName))
                {
                    m_FileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                }
                return m_FileNameWithoutExt;
            }
        }

        public BackupInfo BackupInfo { get; set; }

        public DateTime LastBackupTime { get; set; }
    }
}
