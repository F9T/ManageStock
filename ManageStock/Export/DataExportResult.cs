using System.Collections.Generic;
using System.Linq;

namespace ManageStock.Export
{
    public class DataExportResult
    {
        private int m_NumberImportSuccesValue;

        public DataExportResult()
        {
            m_NumberImportSuccesValue = -1;
            Data = new List<DataExport>();
        }

        public List<DataExport> Data { get; set; }

        public string Result { get; set; }

        public int NumberImportSuccesValue
        {
            get
            {
                if (m_NumberImportSuccesValue == -1)
                {
                    m_NumberImportSuccesValue = Data.Where(_ => _.Reference != null && _.Reference.ExportStatus == EnumDataExportStatus.Sucess).Count();
                }

                return m_NumberImportSuccesValue;
            }
        }
    }
}
