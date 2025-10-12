namespace SmartMealPlanner
{
    public interface IPlannerRepository : IRepository<MealPlanEntry>
    {
        IEnumerable<MealPlanEntry> GetByDate(DateTime date);
        IEnumerable<MealPlanEntry> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}