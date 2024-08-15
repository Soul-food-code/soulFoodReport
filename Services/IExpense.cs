
namespace soulFoodReport.Services {

    public enum ExpenseType { Food, Drink, Design, Decoration, Stationary, Equipment, Utensils, Other }
    public interface IExpense : IMovement {
        ExpenseType ExpenseType {get;}
        string Supplies {get;}
        string  Description {get;}
    }
    public static class Expense {
        public static IExpense Create(IMovement movement, ExpenseType type,string supplies, string description) => new DefaultExpense(movement, type, supplies, description);
    }

    public class DefaultExpense : IExpense
    {
        public DefaultExpense(IMovement movement, ExpenseType expenseType,string supplies, string description)
        {
            Date = movement.Date;
            Amount = movement.Amount;
            Source = movement.Source;
            Type = movement.Type;
            ExpenseType = expenseType;
            Supplies = supplies;
            Description = description;
        }

        public DateTime Date { get; }
        public decimal Amount { get; }
        public ExpenseType ExpenseType {get;}
        public string Supplies {get;}
        public string  Description {get;}
        public SourceType Source {get;}
        public MovementType Type {get;}
    }

    public static class ExpenseHelper {
        public static string GetFileName(this IExpense expense)
            => $"{expense.Date.ToString("yyyy-MM-dd_HH-mm-ss")}_{expense.Type}_EXP.json";
    }
}
