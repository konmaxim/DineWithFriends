using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieCounter.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        

    }
}
