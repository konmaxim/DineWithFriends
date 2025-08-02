using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalorieCounter.Data;
using CalorieCounter.Models;

namespace CalorieCounter.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;

        public DeleteModel(CalorieCounter.Data.DishDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;
        public List<DishImage> DishImages { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null)
            {
                return NotFound();
            }
            else
            {
                Dish = dish;
                DishImages = await _context.DishImages
               .Where(img => img.DishID == Dish.Id)
               .ToListAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FindAsync(id);
//Inefficient plaster written at 2am, optimize later
            var dishImages = await _context.DishImages
    .Where(img => img.DishID == id)
    .ToListAsync();
            if (dish != null && dishImages != null)
            {
                Dish = dish;
                _context.Dishes.Remove(Dish);
                _context.DishImages.RemoveRange(dishImages);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
