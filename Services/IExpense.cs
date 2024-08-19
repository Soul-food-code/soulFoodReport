
using Newtonsoft.Json;

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
        [JsonConstructor]
        public DefaultExpense(DateTime date, decimal amount, MovementType type,SourceType source,ExpenseType expenseType,string supplies, string description) {
            Date = date;
            Amount = amount;
            Source = source;
            Type = type;
            ExpenseType = expenseType;
            Supplies = supplies;
            Description = description;

        }
        public DefaultExpense(IMovement movement, ExpenseType expenseType,string supplies, string description):this(movement.Date,movement.Amount,movement.Type,movement.Source,expenseType,supplies,description)
        {
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
