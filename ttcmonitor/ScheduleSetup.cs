using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TtcMonitor
{
    public class ScheduleSetup
    {
        public int ItemId { get; private set; }
        public int NumberOfPages { get; private set; }
        public int Interval { get; private set; }

        public ScheduleSetup(int itemId, int numberOfPages, int interval)
        {
            ItemId = itemId;
            NumberOfPages = numberOfPages;
            Interval = interval;
        }
    }
}
