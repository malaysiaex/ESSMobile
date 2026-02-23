using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESSMobile.Models
{
    public class DateRange
    {
        public int DateRangeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }   // time
        public TimeSpan EndTime { get; set; }     // time

        public DateRange() { }
        public DateRange(int id, DateTime startdate, DateTime enddate, TimeSpan starttime, TimeSpan endtime)
        {
            DateRangeID = id;
            StartDate = startdate;
            EndDate = enddate;
            StartTime = starttime;
            EndTime = endtime;
        }

    }
}
