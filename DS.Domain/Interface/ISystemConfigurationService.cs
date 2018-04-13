namespace DS.Domain.Interface
{
    public interface ISystemConfigurationService
    {

        SystemConfigurationObject GetSystemConfigurations();

        //SystemConfigurationViewModel GetSystemConfigurationsViewModel();

        SystemConfigurationObject UpdateSystemConfigurations(SystemConfigurationObject model);
        string AppSettings(string appKey);
        int AppSettingsInt(string appKey);
    }
}