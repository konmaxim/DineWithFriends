using System.ComponentModel.DataAnnotations;

namespace CalorieCounter.Models
{
    public class DishImage
    {
        [Key]
        public int DishImageId { get; set; }
        public int DishID { get; set; }
        public string ImageString { get; set; }
        public string ImageUrl { get; set; }


    }
}
