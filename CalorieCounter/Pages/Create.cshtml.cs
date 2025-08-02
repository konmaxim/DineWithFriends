using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CalorieCounter.Data;
using CalorieCounter.Models;
using Microsoft.Extensions.Hosting;

namespace CalorieCounter.Pages
{
    public class CreateModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel( CalorieCounter.Data.DishDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DishViewModel Input { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dish = new Dish
            {
                Name = Input.Name,
                Description = Input.Description
            };
            _context.Add(dish);
            await _context.SaveChangesAsync();
            var lastDishId = dish.Id;
            foreach (var file in Input.Image)
            {
                if (file != null && file.Length > 0)
                {

                    var filePath = Path.Combine(_environment.WebRootPath, "Images/", file.FileName) ;

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
            return RedirectToPage("./Index"); ;
        }
    }
}
