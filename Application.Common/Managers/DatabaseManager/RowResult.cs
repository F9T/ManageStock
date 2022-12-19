namespace Application.Common.Managers.DatabaseManagerBase
{
    public class RowResult
    {
        public RowResult()
        {
            ColumnsList = new ColumnsList();
        }

        public ColumnsList ColumnsList { get; set; }
    }
}
