using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Backup
{
    public class BackupManager
    {
        private static BackupManager s_InstanceOf;

        private List<Backup> m_Backup;
        private bool m_IsBackupRunning;
        private Task m_BackupTask;
        private object m_Lock = new object();

        private BackupManager()
        {
            m_Backup = new List<Backup>();
        }

        public static BackupManager InstanceOf => s_InstanceOf ?? (s_InstanceOf = new BackupManager());

        public void ClearAllBackup()
        {
            lock (m_Lock)
            {
                foreach (Backup backup in m_Backup)
                {
                    DirectoryInfo backupDirectory = new DirectoryInfo(backup.BackupInfo.BackupDirectory);
                    var backupFiles = backupDirectory.GetFiles($"{backup.FileNameWithoutExt}_*.bak", SearchOption.TopDirectoryOnly);
                    foreach(var file in backupFiles)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        public void AddBackup(string _FilePath, BackupInfo _BackupInfo)
        {
            Backup backup = new Backup
            {
                FileName = _FilePath,
                BackupInfo = _BackupInfo
            };

            lock (m_Lock)
            {
                m_Backup.Add(backup);
            }
        }

        public void Start()
        {
            if (m_IsBackupRunning)
            {
                return;
            }

            m_IsBackupRunning = true;
            m_BackupTask = Task.Factory.StartNew(BackupTask);
        }

        public void Stop()
        {
            m_IsBackupRunning = false;
        }

        private void BackupTask()
        {
            while (m_IsBackupRunning)
            {
                lock (m_Lock)
                {
                    DateTime now = DateTime.Now;
                    foreach (Backup backup in m_Backup)
                    {
                        if (now.Subtract(backup.LastBackupTime) >= TimeSpan.FromSeconds(backup.BackupInfo.Interval))
                        {
                            try
                            {
                                if (!File.Exists(backup.FileName))
                                {
                                    continue;
                                }

                                DirectoryInfo backupDirectory = new DirectoryInfo(backup.BackupInfo.BackupDirectory);
                                if (!backupDirectory.Exists)
                                {
                                    backupDirectory.Create();
                                }

                                var backupFiles = backupDirectory.GetFiles($"{backup.FileNameWithoutExt}_*.bak", SearchOption.TopDirectoryOnly).OrderBy(_ => _.LastWriteTime);

                                if (backupFiles.Count() == backup.BackupInfo.MaximumBackup)
                                {
                                    // remove oldest backup
                                    backupFiles.First().Delete();
                                }

                                File.Copy(backup.FileName, Path.Combine(backup.BackupInfo.BackupDirectory, $"{backup.FileNameWithoutExt}_{DateTime.Now:ddMMyyyy_HHmmss}.bak"));

                                backup.LastBackupTime = now;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
