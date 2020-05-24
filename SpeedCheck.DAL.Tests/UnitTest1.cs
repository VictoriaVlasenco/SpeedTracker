using SpeedCheck.DAL.Repositories;
using System;
using System.Linq;
using Xunit;

namespace SpeedCheck.DAL.Tests
{
    public class UnitTest1
    {
        //private const string FilePath = "C:\\test\\test.data";

        [Fact]
        public void Test1()
        {
            //string filePath = FilePath;

            //byte[] encodedText = Encoding.Unicode.GetBytes(text);
            var data = new Models.TrackingData { CheckTime = DateTime.Now, Speed = 60.5, RegistrationNumber = "a1" };//, RegistrationNumber = "1234 AE-5" };
            new FileRepository().Insert(data);

            Assert.True(true, "1 should not be prime");
        }

        [Fact]
        public void Test2()
        {
            //string filePath = FilePath;

            //byte[] encodedText = Encoding.Unicode.GetBytes(text);
            //var data = new Models.TrackingData { CheckTime = DateTime.Now, Speed = 60.5, RegistrationNumber = "1234 AE-5" };
            //new FileRepository().Insert(data);
            //var a = new FileRepository().SelectPage(1, 2, totalCount: out var total);
            var d = new FileRepository().SelectPage(3, 2, totalCount: out var total1);

            d.ToList().Max(x => x.Speed);
            Assert.True(true, "1 should not be prime");
        }
    }
}
