using System;
using System.Diagnostics.CodeAnalysis;

namespace SpeedCheck.BusinessLogic.Models
{
    public class TrackingData : IComparable<TrackingData>
    {
        public DateTime CheckTime { get; set; }

        public string RegistrationNumber { get; set; }

        public double Speed { get; set; }

        public int CompareTo([AllowNull] TrackingData other)
        {
            if (other == null) return 1;

            return Speed.CompareTo(other.Speed);
        }
    }
}
