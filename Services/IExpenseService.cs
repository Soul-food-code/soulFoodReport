using soulFoodReport.Persistency;

namespace soulFoodReport.Services {

    public interface IExpenseService {
        bool Add(IExpense expense);
        IEnumerable<IExpense> GetExpenses((int Year,int Month) period);
        IEnumerable<IExpense> GetExpenses(DateOnly fromDate,DateOnly toDate);

    }

    public class DefaultExpenseService : IExpenseService
    {
        public bool Add(IExpense expense) {
            try {
                return ExpensesPersistency.Save(expense);
            }
            catch (Exception ex) {
                Console.Error.WriteLine("Error on IExpenseService.Add " + expense + " error:" + ex);
                return false;
            }
        }
        public IEnumerable<IExpense> GetExpenses((int Year,int Month) period)  => ExpensesPersistency.LoadExpenses(period);
        public IEnumerable<IExpense> GetExpenses(DateOnly fromDate,DateOnly toDate) => ExpensesPersistency.LoadExpenses(fromDate,toDate);
    
    }
}