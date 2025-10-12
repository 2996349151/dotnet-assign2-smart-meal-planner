namespace SmartMealPlanner
{
    public interface IPantryRepository : IRepository<Ingredient>
    {
        Ingredient GetByTitle(string title);
    }
}