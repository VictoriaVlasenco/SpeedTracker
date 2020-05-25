//using SpeedCheck.BusinessLogic;
//using SpeedCheck.DAL.Repositories;
//using System;
//using Xunit;

//namespace SpeedCheck.BLL.Tests
//{
//    public class UnitTest1
//    {
//        [Fact]
//        public void Test1()
//        {
//            using (var r = new FileRepository())
//            {
//                var service = new TrackService(r);
//                var a = service.GetAllSpeedExceeded(new BusinessLogic.Models.TrackingDataQuery { CheckTime = DateTime.Now, MaxSpeed = 60 });
//            }
//        }

//        [Fact]
//        public void Test2()
//        {
//            using (var r = new FileRepository())
//            {
//                var service = new TrackService(r);
//                var a = service.GetSpeedExtremum(DateTime.Now);
//            }
//        }
//    }
//}
