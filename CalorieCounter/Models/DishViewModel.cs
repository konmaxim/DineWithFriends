namespace CalorieCounter.Models
{
    public class DishViewModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Image { get; set; }
        public string? Ingredients { get; set; }
        public string? Category { get; set; }
    }
}
