using SpeedCheck.DAL.Models;
using SpeedCheck.DAL.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
//using Models = SpeedCheck.BusinessLogic.Models;

namespace SpeedCheck.BusinessLogic
{
    public class TrackService : ITrackService
    {
        private readonly IRepository<TrackingData> repository;

        public TrackService(IRepository<TrackingData> repository)
        {
            this.repository = repository;
        }

        public void Save(Models.TrackingData data)
        {
            this.repository.Insert(new TrackingData() { CheckTime = data.CheckTime, RegistrationNumber = data.RegistrationNumber, Speed = data.Speed });
        }

        public IEnumerable<Models.TrackingData> GetAllSpeedExceeded(Models.TrackingDataQuery query)
        {
            var maxSpeedChunks = new List<Models.TrackingData>();
            var length = this.repository.Count(query.CheckTime.Date);
            var pageSize = 1;

            for (int i = 0; i < length / pageSize + 1; i++)
            {
                var page = this.repository.SelectPage(query.CheckTime, i + 1, pageSize, out var total);
                var res = page
                    .Where(x => x.Speed > query.MaxSpeed)
                    .Select(x => new Models.TrackingData
                    {
                        Speed = x.Speed,
                        CheckTime = x.CheckTime,
                        RegistrationNumber = x.RegistrationNumber
                    });

                maxSpeedChunks.AddRange(res);
            }

            return maxSpeedChunks;
        }

        public (Models.TrackingData Min, Models.TrackingData Max) GetSpeedExtremum(DateTime date)
        {
            var maxSpeedChunks = new List<(Models.TrackingData Min, Models.TrackingData Max)>();
            var length = this.repository.Count(date);
            var pageSize = 3;

            for (int i = 0; i < length / pageSize + 1; i++)
            {
                var page = this.repository.SelectPage(date, i + 1, pageSize, out var total)
                    .Select(x => new Models.TrackingData
                    {
                        Speed = x.Speed,
                        CheckTime = x.CheckTime,
                        RegistrationNumber = x.RegistrationNumber
                    });

                if (page.Count() < 1)
                {
                    break;
                }
                var max = page.Max();
                var min = page.Min();

                maxSpeedChunks.Add((min, max));
            }

            return (maxSpeedChunks.Min(x => x.Min), maxSpeedChunks.Max(x => x.Max));
        }
    }
}
