using System;

namespace Application.Common.Helpers
{
    public static class DateTimeHelper
    {
        public static bool IsDateInRange(this DateTime _TargetDateTime, DateTime _StartDateTime, DateTime _EndDateTime)
        {
            return _TargetDateTime.Ticks >= _StartDateTime.Ticks && _TargetDateTime.Ticks <= _EndDateTime.Ticks;
        }
    }
}
