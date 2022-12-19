using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Application.Common.Managers.DatabaseManagerBase.Conditions
{
    internal class ConditionGroup : ConditionBase
    {
        public ConditionGroup()
        {
            Conditions = new List<ConditionBase>();
        }

        public List<ConditionBase> Conditions { get; set; }

        public List<SQLiteParameter> GetParameters()
        {
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            foreach (ConditionBase conditionBase in Conditions)
            {
                if (conditionBase is Condition condition)
                {
                    parameters.Add(condition.GetParameter());
                }
                else if (conditionBase is ConditionGroup group)
                {
                    parameters.AddRange(group.GetParameters());
                }
            }

            return parameters;
        }
        
        public override string ToString()
        {
            StringBuilder query = new StringBuilder("(");
            for (var index = 0; index < Conditions.Count; index++)
            {
                ConditionBase conditionBase = Conditions[index];
                query.Append(conditionBase);

                if (index + 1 < Conditions.Count)
                    query.Append(" ");
            }
            
            query.Append(")");
            return query.ToString();
        }
    }
}
