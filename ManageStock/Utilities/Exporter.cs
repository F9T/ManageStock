using Application.Common.Models.Articles;
using Application.Common.Models.History;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ManageStock.Utilities
{
    public class Exporter
    {
        private static Exporter s_Instance;

        private Exporter()
        {

        }

        public static Exporter InstanceOf => s_Instance ?? (s_Instance = new Exporter());

        private string ConvertArticleHistoryToExcelFormat(Article _Article)
        {
            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Date;Production;Restockage;Entrée;Sortie;Restant");

            foreach(History history in _Article.History.OrderByDescending(_ => _.Date))
            {
                csv.Append($"{history.Date:dd.MM.yyyy HH:mm:ss};");

                switch (history.ActionType)
                {
                    case EnumStockAction.Production:
                        csv.Append($"{history.Quantity};;;;");
                        break;
                    case EnumStockAction.Resupply:
                        csv.Append($";{history.Quantity};;;");
                        break;
                    case EnumStockAction.Input:
                        csv.Append($";;{history.Quantity};;");
                        break;
                    case EnumStockAction.Output:
                        csv.Append($";;;{history.Quantity};");
                        break;
                }

                csv.AppendLine($"{history.Balance}");
            }

            return csv.ToString();
        }

        public bool ExportArticleHistoryToExcel(string _Path, Article _Article)
        {
            try
            {
                string csv = ConvertArticleHistoryToExcelFormat(_Article);

                using (TextWriter writer = new StreamWriter(_Path, false, Encoding.UTF8))
                {
                    writer.Write(csv);
                    writer.Flush();
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
