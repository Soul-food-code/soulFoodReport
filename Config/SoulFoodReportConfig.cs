using System.Configuration;
using soulFoodReport.Persistency;

namespace soulFoodReport.Config
{
    public interface ISoulFoodReportConfig
    {
        string PersistencyMainFolder { get; }
        string[] Supplies { get; }
        string[] ExpensesTypes { get; }
    }
    public class SoulFoodReportConfig
    {
        public static ISoulFoodReportConfig Instance = new DefaultSoulFoodReportConfig();
        public static void SetConfigurationManger(ConfigurationManager configuration)
        {
            _configurationManager = configuration;
        }
        private static ConfigurationManager? _configurationManager;
        private sealed class DefaultSoulFoodReportConfig : ISoulFoodReportConfig
        {
            public string PersistencyMainFolder { get; } = "/data/soulFoodMovs"; // default volume target
            public string[] Supplies => ExpensesConfigPersistency.LoadSupplies().ToArray();
            public string[] ExpensesTypes => ExpensesConfigPersistency.LoadExpTypes().ToArray();
        }
    }
}