using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using soulFoodReport.Services;

namespace soulFoodReport.Persistency {

    public class ExpensesPersistency 
    {
        public const string SUB_FOLDER = "Expenses";
        public static bool Save(IExpense expenseDetail)
        {
            try {
                var periodFolder = $"{expenseDetail.Date.Year}-{expenseDetail.Date.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                if (!Directory.Exists(fullPath)) {
                    Directory.CreateDirectory(fullPath);
                }
                File.WriteAllText(Path.Combine(fullPath,expenseDetail.GetFileName()),JsonConvert.SerializeObject(expenseDetail));
                return true;
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
                return false;
            }

        }
        public static bool Delete(IExpense expense)
        {
            try {
                var periodFolder = $"{expense.Date.Year}-{expense.Date.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                var filePath = Path.Combine(fullPath,expense.GetFileName());
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
        public static IEnumerable<IExpense> LoadExpenses((int Year,int Month) period) {
            try {
                Console.Out.WriteLine("LoadExpenses with period: " + period);
                var periodFolder = $"{period.Year}-{period.Month.ToString("00")}";
                var fullPath = Path.Combine(baseFolder,periodFolder);
                if (Directory.Exists(fullPath)) {
                    DirectoryInfo d = new DirectoryInfo(fullPath);
                    FileInfo[] expensesFiles = d.GetFiles("*_EXP.json"); //Getting Text files
                    List<IExpense> expenses = new();
                    foreach(FileInfo file in expensesFiles ) {
                        try {
                            var expense = JsonConvert.DeserializeObject<DefaultExpense>(File.ReadAllText(file.FullName));
                            if (expense != null)
                                expenses.Add(expense);
                        }
                        catch  (Exception ex) {
                            Console.Error.WriteLine("Error parsing expense: " + file.FullName + " " + ex);
                        }
                    }
                    Console.Out.WriteLine("LoadExpenses with period: " + period + " movements# " + expenses.Count());

                    return expenses;
                }
                else {
                    Console.Error.WriteLine("No expenses found on " + fullPath);
                    return Enumerable.Empty<IExpense>();
                }
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
            }
            return Enumerable.Empty<IExpense>();
        }

        public static IEnumerable<IExpense> LoadExpenses(DateOnly fromDate,DateOnly toDate) {
            try {
                var periods = GetPeriods(fromDate,toDate);  
                Console.Out.WriteLine("LoadExpenses, identified periods:" + periods.Length);              
                List<IExpense> expenses = new();
                foreach (var period in periods) {
                    var periodExps = LoadExpenses(period);
                    expenses.AddRange(periodExps);
                }
                Console.Out.WriteLine("LoadExpenses, total expenses:" + expenses.Count());              

                return expenses.Where(m => m.Date >= fromDate.ToDateTime(TimeOnly.Parse("00:00 AM")) && m.Date <= toDate.ToDateTime(TimeOnly.Parse("11:59 PM")));
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex);
            }
            return Enumerable.Empty<IExpense>();
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