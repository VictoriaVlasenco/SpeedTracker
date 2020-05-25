using Microsoft.AspNetCore.Mvc;
using SpeedCheck.Binders;
using System;

namespace SpeedCheck.Models
{
    public class TrackingData
    {
        public DateTime CheckTime { get; set; }

        public string RegistrationNumber { get; set; }

        public double Speed { get; set; }

    }
}
