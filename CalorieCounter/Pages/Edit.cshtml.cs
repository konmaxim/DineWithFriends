using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CalorieCounter.Data;
using CalorieCounter.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;

namespace CalorieCounter.Pages
{
    public class EditModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;
        private readonly IWebHostEnvironment _environment; 

        public EditModel(IWebHostEnvironment environment, CalorieCounter.Data.DishDbContext context)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;
        public List<DishImage> DishImages { get; set; } = default!;
//Insert the images into view model
        [BindProperty]
        public DishViewModel Input {  get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish =  await _context.Dishes.FirstOrDefaultAsync(m => m.Id == id);

            if (dish == null)
            {
                return NotFound();
            }
            Dish = dish;
            //Fetch images 
            DishImages = await _context.DishImages
                .Where(img => img.DishID == Dish.Id)
                .ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteImageAsync(int imageId, int dishId)
        {
            var image = await _context.DishImages.FirstOrDefaultAsync(i => i.DishImageId == imageId);
            if (image != null) { 
                _context.DishImages.Remove(image);
                await _context.SaveChangesAsync();
                

            }
            DishImages = await _context.DishImages.Where(i => i.DishID == Dish.Id).ToListAsync();
            TempData["ToastMessage"] = "Your Image was deleted successfully";
            TempData["ToastType"] = "success";
            
            return RedirectToPage("/Edit" ,new { id = dishId });
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
       
        public async Task<IActionResult> OnPostCreateNewImageAsync(int id)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.Id == id);

         
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
                    var newDishImage = new DishImage
                    {
                        DishID = dish.Id,
                        ImageString = file.FileName,
                        ImageUrl = filePath
                    };
                    
                    _context.DishImages.Add(newDishImage);
                    await _context.SaveChangesAsync();
                    DishImages = await _context.DishImages.Where(i => i.DishID == Dish.Id).ToListAsync();
                }
                
            }
            return RedirectToPage("/Edit", new { id = id });
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }

            var dishInDb = await _context.Dishes.FindAsync(Dish.Id);
            
            dishInDb.Name = Dish.Name;
            dishInDb.Description = Dish.Description;
            await _context.SaveChangesAsync();
            DishImages = await _context.DishImages
                .Where(img => img.DishID == Dish.Id)
                .ToListAsync();
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(Dish.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (DishImages != null) {
                return RedirectToPage("./Index");
            }
            return Page();
            
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
