using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Managers.DatabaseManagerBase
{
    public class ColumnsList : List<ColumnResult>
    {
        public object this[string _ColumnName]
        {
            get
            {
                return this.FirstOrDefault(_ => _.ColumnName == _ColumnName);
            }
        }
    }
}
