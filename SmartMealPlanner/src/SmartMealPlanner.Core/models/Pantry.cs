using System.Collections.Generic;

namespace SmartMealPlanner.Core.Models
{
    public sealed class Pantry
    {
        public Dictionary<string, double> Items { get; set; } = new Dictionary<string, double>();
    }
}
