using Simulation_042.Models.Common;

namespace Simulation_042.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Chef> Chefs { get; set; } = [];
    }
}
