using System.Collections.Generic;

namespace SmartMealPlanner.Core.Models
{
    public sealed class UserPreference
    {
        public List<string> DietTags { get; set; } = new List<string>();
        public List<string> Disliked { get; set; } = new List<string>();
        public Dictionary<string, double> CuisineWeights { get; set; } = new Dictionary<string, double>(); // e.g., "italian": 1.2
        public int MissingTolerance { get; set; } = 2;
    }
}
