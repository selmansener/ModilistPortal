using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Models
{
    public class DateTimeRange : ValueObject<DateTimeRange>
    {
        public DateTimeRange()
        {
        }

        public DateTimeRange(DateTime start, DateTime end)
        {
            if (end.CompareTo(start) <= 0)
            {
                throw new InvalidOperationException($"EndTime ({end}) must be greater than StartTime ({start})");
            }

            Start = start;
            End = end;
            Duration = end - start;
        }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public TimeSpan Duration { get; private set; }
    }
}
