using System;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace DS.Tests.ServicesTests
{
    [TestClass]
    public class SystemConfigurationServiceTests
    {
        private IRepository<SystemConfigurationObject> _repository;
        private ICacheProvider _fakecacheprovider;

        [TestInitialize]
        public void TestInit()
        {
            _repository = new MemoryRepository<SystemConfigurationObject>();
            _fakecacheprovider = new FakeCacheProvider();
            ObjectFactory.Initialize(
                x => { x.For<IUnitOfWorkFactory>().Use<MemoryUnitOfWorkFactory>(); }
                );
        }

        [TestMethod]
        public void GetSystemConfiguration_when_cache_provider_is_not_null_and_cache_value_is_not_null_return_cache_value()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject(){SettingId =2};
            _fakecacheprovider.Set("GetSystemConfigurations", sysconfig);
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.GetSystemConfigurations();
            //assert
            Assert.AreEqual(sysconfig, result);
            //cleanup
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }


        [TestMethod]
        public void GetSystemConfiguration_returns_systemconfiguration_setting_id_1()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.GetSystemConfigurations();
            //assert
            Assert.AreEqual(sysconfig, result);
            //cleanup
            _repository.Delete(sysconfig);
        }

        [TestMethod]
        public void GetSystemConfiguration_when_no_result_in_cache_sets_systemconfiguration_setting_id_1_in_cache()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            mut.GetSystemConfigurations();
            SystemConfigurationObject resultObj;
            _fakecacheprovider.Get("GetSystemConfigurations", out resultObj);
            //assert
            Assert.AreEqual(sysconfig, resultObj);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_when_cacheprovider_is_not_null_sets_updatedsystemconfiguration_setting_id_1_in_cache()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() {SettingId = 1, CouncilName = "test"};
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            mut.UpdateSystemConfigurations(sysConfigToUpdate);
            SystemConfigurationObject resultObj;
            _fakecacheprovider.Get("GetSystemConfigurations", out resultObj);
            //assert
            Assert.AreEqual(sysconfig, resultObj);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }


        [TestMethod]
        public void UpdateSystemConfiguration_updates_analytics_tracking_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, AnalyticsTrackingRef = "analytics" };
            
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);
            
            //assert
            Assert.AreEqual("analytics", result.AnalyticsTrackingRef);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_CouncilName_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, CouncilName = "council" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("council", result.CouncilName);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_CouncilUri_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, CouncilUri = "uri" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("uri", result.CouncilUri);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_CouncilUrl_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, CouncilUrl = "url" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("url", result.CouncilUrl);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_CouncilSpatialUri_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, CouncilSpatialUri = "spatial" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("spatial", result.CouncilSpatialUri);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_latitude_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, MapCentreLatitude = "lat" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("lat", result.MapCentreLatitude);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void UpdateSystemConfiguration_updates_longitude_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, MapCentreLongitude = "long" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("long", result.MapCentreLongitude);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }
        [TestMethod]
        public void UpdateSystemConfiguration_updates_defaultzoom_reference_in_repository()
        {

            //arrange
            var sysconfig = new SystemConfigurationObject() { SettingId = 1 };
            _repository.Add(sysconfig);
            var sysConfigToUpdate = new SystemConfigurationObject() { SettingId = 1, MapDefaultZoom = "12" };

            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act
            var result = mut.UpdateSystemConfigurations(sysConfigToUpdate);

            //assert
            Assert.AreEqual("12", result.MapDefaultZoom);
            //cleanup
            _repository.Delete(sysconfig);
            _fakecacheprovider.Clear("GetSystemConfigurations");
        }

        [TestMethod]
        public void AppSettings_Returns_value_from_app_settings_Config_file()
        {
            //arrange
            var appKey = "ForUnitTestReturnKey";
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act

            var result = mut.AppSettings(appKey);
            //assert
           
            Assert.AreEqual("ForUnitTestReturnValue", result);
            //cleanup
        }
        [TestMethod]
        public void AppSettings_When_not_found_returns_text_Returns_please_enter_keyvalue_in_config_file()
        {
            //arrange
            var appKey = "ForUnitTestNotInAppConfigFile";
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act

            var result = mut.AppSettings(appKey);
            //assert

            Assert.AreEqual("PleaseEnterKeyValueInConfigFile", result);
            //cleanup
        }
        [TestMethod]
        public void AppSettingsInt_returns_int_Returns_converted_value_config_file()
        {
            //arrange
            var appKey = "ForUnitTestIntReturnKey";
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act

            var result = mut.AppSettingsInt(appKey);
            //assert

            Assert.AreEqual(1024, result);
            //cleanup
        }
        [TestMethod]
        public void AppSettingsInt_When_no_key_found_returns_int_Returns_0()
        {
            //arrange
            var appKey = "ForUnitTestIntReturnKeyNotThere";
            var mut = new SystemConfigurationService(_repository, _fakecacheprovider);
            //act

            var result = mut.AppSettingsInt(appKey);
            //assert

            Assert.AreEqual(0, result);
            //cleanup
        }
    }
}
