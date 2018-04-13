using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using StructureMap;

namespace DS.Service
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private IRepository<SystemConfigurationObject> _dataRepository;
        private ICacheProvider _cacheProvider;


        public SystemConfigurationService(IRepository<SystemConfigurationObject> dataRepository, ICacheProvider cacheProvider)
        {
            _dataRepository = dataRepository;
            _cacheProvider = cacheProvider;
        }

        public SystemConfigurationObject GetSystemConfigurations()
        {
             SystemConfigurationObject cachedValue =null;
            if (_cacheProvider != null)
                _cacheProvider.Get("GetSystemConfigurations", out cachedValue);

            if (cachedValue != null) return cachedValue;

            
            cachedValue = _dataRepository.Query(x => x.SettingId == 1).First();    
            _cacheProvider.Set("GetSystemConfigurations",cachedValue, 1);
            
            return cachedValue;
        }


        public SystemConfigurationObject UpdateSystemConfigurations(SystemConfigurationObject model)
        {
            /*here to check the minimum values have been put in*/


            var dbObj = _dataRepository.Query(x => x.SettingId ==  1).First();
            dbObj.AnalyticsTrackingRef = model.AnalyticsTrackingRef;
            dbObj.CouncilName = model.CouncilName;
            dbObj.CouncilUri = model.CouncilUri;
            dbObj.CouncilUrl = model.CouncilUrl;
            dbObj.CouncilSpatialUri = model.CouncilSpatialUri;
            dbObj.MapCentreLatitude = model.MapCentreLatitude;
            dbObj.MapCentreLongitude = model.MapCentreLongitude;
            dbObj.MapDefaultZoom = model.MapDefaultZoom;
            dbObj.SendEmailForFeedback = model.SendEmailForFeedback;
            dbObj.SmtpServer = model.SmtpServer;
            dbObj.SmtpUsername = model.SmtpUsername;
            dbObj.SmtpPassword = model.SmtpPassword;
            _dataRepository.SaveChanges();
            
            if (_cacheProvider != null)
                _cacheProvider.Set("GetSystemConfigurations", dbObj);

            return dbObj;
        }

        public string AppSettings(string appKey)
        {
            return ConfigurationManager.AppSettings[appKey] ?? "PleaseEnterKeyValueInConfigFile";
        }

        public int AppSettingsInt(string appKey)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings[appKey]);
        }
    }
}
