namespace CalorieCounter.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public int DishId { get; set; }   // FK
        public Dish Dish { get; set; } = null!;
    }
}
