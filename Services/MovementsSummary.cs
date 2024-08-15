
namespace soulFoodReport.Services {

    public interface IMovementsSummary {
        DateOnly FromDate {get;}
        DateOnly ToDate {get;}
        decimal CashAmount {get;}
        decimal CardAmount {get;}

        IEnumerable<IDaySummary> DaySummaries {get;}
    }

    public interface IDaySummary {
        DateOnly Date {get;}
        decimal CashAmount {get;}
        decimal CardAmount {get;}
    }

    public static class MovementSummary {
        public static IMovementsSummary Create(IEnumerable<IMovement> movements) => new DefaultMovementsSummary(movements);
        public static IDaySummary CreateDay(DateOnly date,decimal cashAmount,decimal cardAmount) => new DefaultDaySummary(date,cashAmount,cardAmount);
    }

    public class DefaultMovementsSummary : IMovementsSummary
    {
        public DefaultMovementsSummary(IEnumerable<IMovement> movements)
        {
            var orderedMovements = movements.OrderBy(m => m.Date).ToArray();
            if (orderedMovements.Length > 0) {

                FromDate = DateOnly.FromDateTime(orderedMovements.First().Date);
                ToDate = DateOnly.FromDateTime(orderedMovements.Last().Date);
                var daySummaries = new List<IDaySummary>();
                var cashAmount = 0m;
                DateOnly currDate = FromDate;
                while (currDate <= ToDate) {
                    Console.Out.WriteLine("DefaultMovementsSummary handling date:" + currDate);
                    var dayMovements = movements.Where(m => DateOnly.FromDateTime(m.Date) == currDate).ToArray();
                    Console.Out.WriteLine("DefaultMovementsSummary handling date:" + currDate + " dayMovements#: " + dayMovements.Length);

                    var openDrawerMov  = dayMovements.Where(m => m.Source == SourceType.Drawer && m.Type == MovementType.Open).OrderBy(m => m.Date).ToArray();
                    var closeDrawerMov = dayMovements.Where(m => m.Source == SourceType.Drawer && m.Type == MovementType.Close).OrderBy(m => m.Date).ToArray();
                    Console.Out.WriteLine("DefaultMovementsSummary handling date:" + currDate + " openDrawerMov#: " + openDrawerMov.Length + " closeDrawerMov#:" + closeDrawerMov.Length);

                    if (openDrawerMov.Length != closeDrawerMov.Length) {
                        Console.Error.WriteLine(currDate + " drawer hasn't been closed properly");
                    }
                    else {
                        for (int i=0;i<openDrawerMov.Length;i++) {
                            var cashDiff = closeDrawerMov[i].Amount - openDrawerMov[i].Amount;
                            Console.Out.WriteLine("DefaultMovementsSummary handling date:" + currDate + " close: " + closeDrawerMov[i].Amount + " open:" + openDrawerMov[i].Amount);

                            cashAmount += cashDiff;
                            var cardAmount =  dayMovements.Where(m => m.Source == SourceType.Card).Sum(m => m.Amount);
                            daySummaries.Add(MovementSummary.CreateDay(currDate,cashAmount,cardAmount));
                        }
                    }


                    currDate = currDate.AddDays(1);
                }
                CashAmount = cashAmount;
                CardAmount = orderedMovements.Where(m => m.Source == SourceType.Card && m.Type == MovementType.Deposit).Sum(m => m.Amount);
                DaySummaries = daySummaries;
            }
        }

        public DateOnly FromDate {get;}
        public DateOnly ToDate {get;}
        public decimal CashAmount {get;}
        public decimal CardAmount {get;}
        public IEnumerable<IDaySummary> DaySummaries {get;} = Enumerable.Empty<IDaySummary>();
    }

    public class DefaultDaySummary(DateOnly date,decimal cashAmount,decimal cardAmount) : IDaySummary {
        public DateOnly Date {get;} = date;
        public decimal CashAmount {get;} = cashAmount;
        public decimal CardAmount {get;} = cardAmount;
    }

}
