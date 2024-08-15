using System.Configuration;

public interface ISoulFoodReportConfig {
    string PersistencyMainFolder {get;}
}
public class SoulFoodReportConfig {
    public static ISoulFoodReportConfig Instance = new DefaultSoulFoodReportConfig();
    public static void SetConfigurationManger(ConfigurationManager configuration) {
        _configurationManager = configuration;
    }
    private static ConfigurationManager? _configurationManager;
    private class DefaultSoulFoodReportConfig : ISoulFoodReportConfig
    {
        public string PersistencyMainFolder => _configurationManager.GetValue<string>("PersistencyMainFolder");

    }
}