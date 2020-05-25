using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SpeedCheck.BusinessLogic;
using SpeedCheck.BusinessLogic.Models;
using SpeedCheck.Filters;

namespace SpeedCheck.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SpeedController : ControllerBase
    {
        private readonly ITrackService service;

        public SpeedController(ITrackService service)
        {
            this.service = service;
        }

        [HttpPost]
        public void Save([FromBody]Models.TrackingData data)
        {
            this.service.Save(new TrackingData { CheckTime = data.CheckTime, RegistrationNumber = data.RegistrationNumber, Speed = data.Speed });
        }

        [HttpGet]
        [Route("GetExtremum")]
        [TypeFilter(typeof(QueryTimeActionFilter))]
        public (TrackingData Min, TrackingData Max) GetExtremum(DateTime date)
        {
            var res = this.service.GetSpeedExtremum(date);

            return res;
        }

        [HttpGet]
        [Route("GetExceeded")]
        [TypeFilter(typeof(QueryTimeActionFilter))]
        public IEnumerable<TrackingData> Get(DateTime date, double speed)
        {
            var res = this.service.GetAllSpeedExceeded(new TrackingDataQuery { CheckTime = date, MaxSpeed = speed });

            return res;
        }


    }
}
