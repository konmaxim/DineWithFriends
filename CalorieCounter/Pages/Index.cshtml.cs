using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalorieCounter.Data;
using CalorieCounter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CalorieCounter.Pages
{
   
    public class IndexModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(UserManager<User> userManager, CalorieCounter.Data.DishDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public string ProfilePictureUrl { get; private set; } = "";
        public IList<DishViewModel> Dish { get;set; } = default!;
        public List<DishImage> DishImages { get; set; } = default!;
        public List<int> DishIds = []; 
        public async Task OnGetAsync()
        {
           
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID != null)
            {
                var user = await _userManager.FindByIdAsync(userID);
                ProfilePictureUrl = user.ProfilePicturePath;
            }
            Dish = await _context.Dishes
            .Where(d => d.User.Id == userID)
            .Select(d => new DishViewModel
            {
         
                Name = d.Name,
                Description = d.Description,
                Ingredients = d.Ingredients,
                Category = d.Category

            })
            .ToListAsync();
            DishIds = _context.Dishes
                      .Where(d => d.User.Id == userID)
                      .Select(d => d.Id)
                       .ToList();

            DishImages = await (
                from img in _context.DishImages
                join d in _context.Dishes on img.DishID equals d.Id
                where d.User.Id == userID
                select img
                ).ToListAsync();

        }
    }
}
