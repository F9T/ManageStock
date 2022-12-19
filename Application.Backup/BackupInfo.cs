namespace Application.Backup
{
    public struct BackupInfo
    {
        public BackupInfo(string _BackupDirectory, uint _Interval, ushort _MaximumBackup)
        {
            BackupDirectory = _BackupDirectory;
            Interval = _Interval;
            MaximumBackup = _MaximumBackup;

            if (Interval < 10)
            {
                Interval = 10;
            }
        }

        // in seconds
        public uint Interval { get; }
    
        public ushort MaximumBackup { get; }

        public string BackupDirectory { get; }
    }
}
