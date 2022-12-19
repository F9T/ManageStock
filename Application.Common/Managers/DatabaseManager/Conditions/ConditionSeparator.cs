namespace Application.Common.Managers.DatabaseManagerBase.Conditions
{
    internal class ConditionSeparator : ConditionBase
    {
        public ConditionSeparator(EnumConditionSeparator separator)
        {
            Separator = separator;
        }

        public EnumConditionSeparator Separator { get; }

        public override string ToString()
        {
            return Separator.ToString();
        }
    }
}
