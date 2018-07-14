using System;
using Info.Zainco.ZamTrack.PersistenceManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Info.Zainco.ZamTrack.Unit.Tests
{
    [TestClass]
    public class GpsDataPointParserTest
    {
        private GpsDataPointParser _dataPointParser;

        [TestInitialize]
        public void SetUp()
        {
            _dataPointParser = new GpsDataPointParser("%%800704,A,110402201650,N5121.3489W00011.1780,000,230,NA,47000000,531,CFG:Z31,10,1|");           
        }

        [TestMethod]
        public void TestCalculateLatitudeLongitude()
        {
            Assert.AreEqual(51.355815m, _dataPointParser.Position.Latitude);
            Assert.AreEqual(-0.18630m, _dataPointParser.Position.Longitude);
        }

        [TestMethod]
        public void TestCalculateSpeed()
        {
            Assert.AreEqual(0,_dataPointParser.Speed,"The expected speed did not match the actual speed");
        }

        [TestMethod]
        public void TestIsValidGps()
        {
            const bool expectedIsGpsValid = true;
            Assert.AreEqual(expectedIsGpsValid, _dataPointParser.IsGpsValid, "The expected IsGpsValid did not match");
        }

        [TestMethod]
        public void TestEventId()
        {
            const decimal eventId = 531m;
            Assert.AreEqual(eventId, _dataPointParser.EventId, "The expected eventId did not match");
        }

        [TestMethod]
        public void TestStatus()
        {
            const long expectedStatus = 47000000l;
            Assert.AreEqual(expectedStatus, _dataPointParser.Status, "The expected status did not match");
        }
        [TestMethod]
        public void TestDirection()
        {
            const int expectedDirection = 230;
            Assert.AreEqual(expectedDirection,_dataPointParser.Direction,"The expected direction did not match");
        }

        [TestMethod]
        public void TestDeviceId()
        {
            const string expectedDeviceId = "800704";
            Assert.AreEqual(expectedDeviceId,_dataPointParser.DeviceId);
        }

        [TestMethod]
        public void TestCalculateDateAndTime()
        {
            var expectedTimeStamp = new DateTime(2011,4,2,20,16,50);
            Assert.AreEqual(expectedTimeStamp, _dataPointParser.GpsTimeStamp);
        }
    }
}
