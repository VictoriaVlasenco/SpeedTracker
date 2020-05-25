using SpeedCheck.BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace SpeedCheck.BusinessLogic
{
    public interface ITrackService
    {
        IEnumerable<TrackingData> GetAllSpeedExceeded(TrackingDataQuery query);

        (TrackingData Min, TrackingData Max) GetSpeedExtremum(DateTime date);

        void Save(Models.TrackingData data);
    }
}