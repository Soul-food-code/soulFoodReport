using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using soulFoodReport.Services;

namespace soulFoodReport.Persistency {

    public class MovementPersistency 
    {
        public const string SUB_FOLDER = "Movements";
        public static bool Save(IMovement movement)
        {
            try {
                var periodFolder = $"{movement.Date.Year}-{movement.Date.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                if (!Directory.Exists(fullPath)) {
                    Directory.CreateDirectory(fullPath);
                }
                File.WriteAllText(Path.Combine(fullPath,movement.GetFileName()),JsonConvert.SerializeObject(movement));
                return true;
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
                return false;
            }

        }
        public static bool Delete(IMovement movement)
        {
            try {
                var periodFolder = $"{movement.Date.Year}-{movement.Date.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                var filePath = Path.Combine(fullPath,movement.GetFileName());
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
                return false;
            }

        }
        
        public static IEnumerable<IMovement> LoadMovements((int Year,int Month) period) {
            try {
                Console.Out.WriteLine("LoadMovements with period: " + period);
                var periodFolder = $"{period.Year}-{period.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                if (Directory.Exists(fullPath)) {
                    DirectoryInfo d = new DirectoryInfo(fullPath);
                    FileInfo[] movementFiles = d.GetFiles("*_MOV.json"); //Getting Text files
                    List<IMovement> movements = new();
                    foreach(FileInfo file in movementFiles ) {
                        try {
                            var movement = JsonConvert.DeserializeObject<DefaultMovement>(File.ReadAllText(file.FullName));
                            if (movement != null)
                                movements.Add(movement);
                        }
                        catch  (Exception ex) {
                            Console.Error.WriteLine("Error parsing movement: " + file.FullName + " " + ex);
                        }
                    }
                    Console.Out.WriteLine("LoadMovements with period: " + period + " movements# " + movements.Count());

                    return movements;
                }
                else {
                    Console.Error.WriteLine("No movements found on " + fullPath);
                    return Enumerable.Empty<IMovement>();
                }
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
            }
            return Enumerable.Empty<IMovement>();
        }

        public static IEnumerable<IMovement> LoadMovements(DateOnly fromDate,DateOnly toDate) {
            try {
                var periods = GetPeriods(fromDate,toDate);  
                Console.Out.WriteLine("LoadMovements, identified periods:" + periods.Length);              
                List<IMovement> movements = new();
                foreach (var period in periods) {
                    var periodMovs = LoadMovements(period);
                    movements.AddRange(periodMovs);
                }
                Console.Out.WriteLine("LoadMovements, total movements:" + movements.Count());              

                return movements.Where(m => m.Date >= fromDate.ToDateTime(TimeOnly.Parse("00:00 AM")) && m.Date <= toDate.ToDateTime(TimeOnly.Parse("11:59 PM")));
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
            }
            return Enumerable.Empty<IMovement>();
        }

        private static (int Year,int Month)[] GetPeriods(DateOnly fromDate,DateOnly toDate) {
            var currDate = fromDate;
            List<(int Year,int Month)> periodFolders = new();
            while (currDate <= toDate) {
                var periodFolder = (currDate.Year,currDate.Month);
                periodFolders.Add(periodFolder);
                Console.Out.WriteLine("Period identified: " + periodFolder);
                currDate = currDate.AddMonths(1);    
            }
            return periodFolders.ToArray();
        }
        private static string mainPersistencyFolder = SoulFoodReportConfig.Instance.PersistencyMainFolder ?? Directory.GetCurrentDirectory();
        private static string baseFolder = Path.Combine(mainPersistencyFolder,SUB_FOLDER);
    }
}