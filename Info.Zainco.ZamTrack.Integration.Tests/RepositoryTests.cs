using System;
using System.Collections.Generic;
using Info.Zainco.ZamTrack.PersistenceManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Info.Zainco.ZamTrack.Integration.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        private Repository _repository;
        [TestCleanup]
        public void CleanUpDatabase()
        {
            Repository.DeleteDataPointsForDevice("800704");
        }

        [TestInitialize]
        public void SetUp()
        {
            _repository = Repository.Factory("%%800704,A,110506180732,N5126.0549W00031.6764,000,051,NA,17800000,108");           
        }

        [TestMethod]
        public void TestGetAllDevices()
        {
            //save 5 devices and get them
            CreateDevices();
            var allDevices = Repository.GetAllDevices();
            Assert.IsTrue(allDevices.Count > 0,"Failed to load GPS devices");
        }

        [TestMethod]
        public void TestGetAllDevicesJson()
        {
            CreateDevices();
            var jsonDeviceData = Repository.GetAllDevicesJson();
            Assert.IsFalse(string.IsNullOrEmpty(jsonDeviceData));  
        }

        [TestMethod]
        public void TestSaveDataPoints()
        {
            Repository.Factory(
                @"%%800704,A,110506191715,N5118.3523W00013.1674,038,013,NA,17800100,108
%%800704,A,110506191723,N5118.4043W00013.1319,060,027,NA,17800100,110
%%800704,A,110506191755,N5118.6953W00012.8776,077,026,NA,17800100,109
%%800704,A,110506191902,N5119.1650W00012.6778,067,008,NA,17800100,110

")
                .SaveDataPoints();
        }

        [TestMethod]
        public void TestJsonDeviceData()
        {
            CreateDevices();
            var jsonDeviceData = Repository.GetGpsPointsForDeviceJson("800704",DateTime.Now.AddDays(-1),DateTime.Now);
            Assert.IsFalse(string.IsNullOrEmpty(jsonDeviceData));  
        }

        [TestMethod]
        public void TestJsonVehiclePathForDevice()
        {
            CreateDevices();
            var jsonVehiclePath = Repository.VehiclePathInJson("800704", DateTime.Now.AddYears(-1), DateTime.Now);
            Assert.IsFalse(string.IsNullOrEmpty(jsonVehiclePath));

        }

        private void CreateDevices()
        {
            for (int i = 0; i < 5; i++)
            {
                _repository.SaveDataPoint();
            }
        }

        [TestMethod]       
        public void TestCreationOfGpsDataPoint()
        {
            Assert.IsTrue(_repository.SaveDataPoint(),"Failed to create a GPS Data point");
        }

        [TestMethod]
        public void TestGetGpsPointsForDevice()
        {
            _repository.SaveDataPoint();
            ICollection<GpsDataPoint> dataPoints =  Repository.GetGpsPointsForDevice("800704",DateTime.Now.AddDays(-2),DateTime.Now);

            Assert.IsTrue(dataPoints.Count > 0);
        }

        
    }
}
