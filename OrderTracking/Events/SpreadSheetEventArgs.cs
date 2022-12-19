using System;

namespace OrderTracking.Events
{
    public class SpreadSheetEventArgs : EventArgs
    {
        public SpreadSheetEventArgs(string _FileName, EnumAction _Action)
        {
            FileName = _FileName;
            Action = _Action;
        }

        public string FileName { get; set; }

        public EnumAction Action { get; set; }
    }

    public enum EnumAction
    {
        Save,
        Load
    }
}
