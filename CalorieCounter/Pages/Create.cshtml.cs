using CalorieCounter.Data;
using CalorieCounter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalorieCounter.Pages
{
    public class CreateModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;


        public CreateModel(CalorieCounter.Data.DishDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult OnGet()
        {

            return Page();

        }

        [BindProperty]
        public DishViewModel Input { get; set; } = new();

        public DishViewModel RecommendedDish { get; set; } = new DishViewModel(); // single dish
        public string reccDishimg = "";

        // For more information, see https://aka.ms/RazorPagesCRUD.
        //
        
        public async Task<IActionResult> OnPostEmbedDishesAsync()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dishTexts = await _context.Dishes
    .Where(d => d.User.Id == userID)
    .Select(d =>
        "Name: " + d.Name +
        ", Category: " + d.Category +
        ", Description: " + d.Description +
        ", Ingredients: " + d.Ingredients)
    .ToListAsync();

            foreach (var text in dishTexts)
            {
                Console.WriteLine(text);
            }
            var json = JsonSerializer.Serialize(dishTexts);
            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5000/Create/embeddishes", content);
            var json1 = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json1);

            // Replace invalid NaN tokens with null
            var safeJson = json1.Replace("NaN", "null");

            // Parse JSON 
            using var doc = JsonDocument.Parse(safeJson);
            var root = doc.RootElement;
            RecommendedDish.Name = root.GetProperty("Name").GetString();
            RecommendedDish.Description = root.GetProperty("Description").GetString();
            RecommendedDish.Category = root.GetProperty("Category").GetString();
            RecommendedDish.Ingredients = root.GetProperty("Ingredients").GetString();
            reccDishimg = root.GetProperty("Image").GetString();
            string html = $@"
            <div class='card shadow border-0'>
            <img src='{reccDishimg}' class='card-img-top' alt='{RecommendedDish.Name}' />
            <div class='card-body'>
                <h4 class='card-title'>{RecommendedDish.Name}</h4>
                <p><strong>Category:</strong> {RecommendedDish.Category}</p>
                <p><strong>Ingredients:</strong> {RecommendedDish.Ingredients}</p>
                <p>{RecommendedDish.Description}</p>
            </div>
        </div>";
            Console.WriteLine("Returning HTML: " + html);
            return Content(html, "text/html");




          
        }



        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dish = new Dish
            {
                Name = Input.Name,
                Description = Input.Description,
                Ingredients = Input.Ingredients,
                Category = Input.Category,
                User = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier))

            };
            _context.Add(dish);
            await _context.SaveChangesAsync();
            var lastDishId = dish.Id;
            if (Input.Image != null)
            {
                foreach (var file in Input.Image)
                {
                    if (file != null && file.Length > 0)
                    {

                        var filePath = Path.Combine(_environment.WebRootPath, "Images/", file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            stream.Close();
                        }

                        var dishImage = new DishImage
                        {
                            DishID = lastDishId,
                            ImageString = file.FileName,
                            ImageUrl = filePath
                        };

                        _context.DishImages.Add(dishImage);
                        await _context.SaveChangesAsync();
                    }
                }
               
            }
            return RedirectToPage("./Index");
        }
    }
}
