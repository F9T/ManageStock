namespace ManageStock.Export
{
    public class DataExportValue
    {
        public DataExportValue()
        {
            ExportStatus = EnumDataExportStatus.NotHandled;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public EnumDataExportStatus ExportStatus { get; set; }
    }
}
