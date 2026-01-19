using Simulation_042.Models.Common;

namespace Simulation_042.Models
{
    public class Chef:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
