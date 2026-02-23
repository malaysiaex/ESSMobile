using GeoTimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESSMobile.Models
{
    public class CompanyLocation
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public double Latitude {get; set;}
        public double Longitude { get; set; }

        // how far client can be from this location to be counted as "near" (km)
        public double? BoundaryDistanceM { get; set; }

        // current distance from client to location (km), calculate on userLocation retrieval
        public double DistanceM { get; set; }
        // optional, default to empty
        public string Address { get; set; }

        // the time periods associated with a company location
        public List<DayOfWeekWindow> CheckInWindows { get; set; }  = new();
        public List<DateRange> AssignedDateRanges { get; set; } = new();
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
    }
}

