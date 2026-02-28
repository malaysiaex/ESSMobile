using GeoTimeZone;

namespace ESSMobile.Models
{
    public class CompanyLocation
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public double Latitude {get; set;}
        public double Longitude { get; set; }

        // how far client can be from this location to be counted as "near" (m)
        public double? BoundaryDistanceM { get; set; }

        // current distance from client to location (m), calculate on userLocation retrieval
        public double DistanceM { get; set; }
        // optional, default to empty
        public string Address { get; set; }

        // the time periods associated with a company location
        public List<DayOfWeekWindow> CheckInWindows { get; set; }  = new();
        public List<DateRange> AssignedDateRanges { get; set; } = new();

        // bools for within window
        public bool withinWeeklyWindow { get; set; }
        public bool withinDateRange { get; set; }


        // timezones 
        public string TimeZoneId { get; set; }
        // cached runtime object 
        private TimeZoneInfo? _timeZoneInfo;


        public CompanyLocation(int id, string name, double latitude, double longitude, double boundaryDistance, string address = "")
        {
            Id = id;
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            BoundaryDistanceM = boundaryDistance;
            Address = address;
            TimeZoneId = GetTimeZoneIdFromCoordinates(latitude, longitude);
        }
        public bool IsNearWorkplace()
        {
            if (BoundaryDistanceM.HasValue)
            {
                return DistanceM <= BoundaryDistanceM;
            }
            return true;
        }

        // time zone stuff
        private string GetTimeZoneIdFromCoordinates(double latitude, double longitude)
        {
            return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        }
        public TimeZoneInfo GetTimeZoneInfo()
        {
            if (_timeZoneInfo == null)
                _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

            return _timeZoneInfo;
        }
        public DateTime GetCompanyLocalTime(DateTime serverUtc)
        {
            serverUtc = DateTime.SpecifyKind(serverUtc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(serverUtc, GetTimeZoneInfo());
        }
        public double GetDistanceFromBoundary()
        {
            if (BoundaryDistanceM.HasValue) {
                return Math.Max(0, this.DistanceM - this.BoundaryDistanceM.Value);       
            }
            return 0;
        }
        /// <summary>
        /// Calculates and stores withinWeeklyWindow and withinDateRange.
        /// </summary>
        /// <param name="companyLocalTime"></param>
        public void CalculateWithinWindows(DateTime companyLocalTime)
        {
            // day of week check
            this.withinWeeklyWindow = this.CheckInWindows
                .Where(w => w.DayOfWeek == (byte)companyLocalTime.DayOfWeek)
                .Any(w =>
                    companyLocalTime.TimeOfDay >= w.StartTime &&
                    companyLocalTime.TimeOfDay <= w.EndTime
                );
            // date range check 
            this.withinDateRange = this.AssignedDateRanges
                .Any(r =>
                    companyLocalTime.Date >= r.StartDate.Date &&
                    companyLocalTime.Date <= r.EndDate.Date &&
                    companyLocalTime.TimeOfDay >= r.StartTime &&
                    companyLocalTime.TimeOfDay <= r.EndTime
                );

        }
    }
}

