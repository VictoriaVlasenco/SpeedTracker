using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using SpeedCheck.BusinessLogic;
using SpeedCheck.BusinessLogic.Models;

namespace SpeedCheck.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SpeedController : ControllerBase
    {
        private readonly ILogger<SpeedController> logger;
        private readonly ITrackService service;

        public SpeedController(ILogger<SpeedController> logger, ITrackService service)
        {
            logger = logger;
            this.service = service;
        }

        [HttpPost]
        public void Save([FromBody]Models.TrackingData data)
        {
            this.service.Save(new TrackingData { CheckTime = data.CheckTime, RegistrationNumber = data.RegistrationNumber, Speed = data.Speed });
        }

        [HttpGet]
        [Route("GetExtremum")]
        public (TrackingData Min, TrackingData Max) GetExtremum(DateTime date)
        {
            var res = this.service.GetSpeedExtremum(date);

            return res;
        }

        [HttpGet]
        [Route("GetExceeded")]
        public IEnumerable<TrackingData> Get(DateTime date, double speed)
        {
            var res = this.service.GetAllSpeedExceeded(new TrackingDataQuery { CheckTime = date, MaxSpeed = speed });

            return res;
        }


    }
}
