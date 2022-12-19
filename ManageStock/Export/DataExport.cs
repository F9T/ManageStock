using System.Collections.Generic;

namespace ManageStock.Export
{
    public class DataExport
    {
        public DataExport()
        {
            Data = new List<DataExportValue>();
        }

        public DataExportValue Reference { get; set; }

        public List<DataExportValue> Data { get; set; }
    }
}
