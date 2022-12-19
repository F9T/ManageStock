using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Application.Excel
{
    public class ExcelManager
    {
        private static ExcelManager s_InstanceOf = null;

        private ExcelManager()
        {

        }

        public static ExcelManager InstanceOf => s_InstanceOf ?? (s_InstanceOf = new ExcelManager());

        public EnumExcelStatus ReadWorksheet(string _ExcelPath, out ExcelWorksheet _ExcelWorksheet)
        {
            if (string.IsNullOrEmpty(_ExcelPath) || !File.Exists(_ExcelPath))
            {
                _ExcelWorksheet = null;
                return EnumExcelStatus.FileNotExist;
            }

            // check is already in use
            try
            {
                using (FileStream fileStream = new FileStream(_ExcelPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // nothing
                }
            }
            catch (IOException)
            {
                _ExcelWorksheet = null;
                return EnumExcelStatus.FileInUse;
            }

            Workbook workbook = null;
            Microsoft.Office.Interop.Excel.Application app = null;
            try
            {
                app = new Microsoft.Office.Interop.Excel.Application();

                workbook = app.Workbooks.Open(_ExcelPath);
                Worksheet worksheet = workbook.Worksheets[1];

                _ExcelWorksheet = new ExcelWorksheet
                {
                    Cells = new string[worksheet.UsedRange.Rows.Count, worksheet.UsedRange.Columns.Count]
                };

                foreach (Range cell in worksheet.UsedRange.Cells)
                {
                    if (cell.Value != null)
                    {
                        _ExcelWorksheet.Cells[cell.Row - 1, cell.Column - 1] = cell.Value?.ToString();
                    }
                }

                workbook.Close();
                app.Quit();

                return EnumExcelStatus.Success;
            }
            catch (Exception)
            {
                try
                {
                    if (workbook != null)
                    {
                        workbook.Close();
                    }
                    if (app != null)
                    {
                        app.Quit();
                    }
                }
                catch (Exception)
                {
                    // nothing
                }
                _ExcelWorksheet = null;
                return EnumExcelStatus.Unknown;
            }
        }
    }
}
