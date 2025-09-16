using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieCounter.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Ingredients { get; set; }
        public string? Category { get; set; }
        public User User { get; set; }

    }
}
