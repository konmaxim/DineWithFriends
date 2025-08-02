using CalorieCounter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CalorieCounter.Pages
{
    public class FriendsModel : PageModel
    {
        private readonly CalorieCounter.Data.DishDbContext _context;
        private readonly UserManager<User> _userManager;
        public FriendsModel(UserManager<User> userManager, CalorieCounter.Data.DishDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public List<Friendship> FriendsList { get; set; } = default!;
        public string ProfilePictureUrl = "";
        [BindProperty]
        public string NewFriendName { get; set; }
        [BindProperty]
       
        public string EmailAddress { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public List<Friendship>? PendingRequestsSent { get; set; }
        public List<Friendship>? PendingRequestsRecieved { get; set; }
        public async Task OnGetAsync()
        {
          
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID != null) {
                var user = await _userManager.FindByIdAsync(userID);
                 ProfilePictureUrl = user.ProfilePicturePath;
                if (user.Inviters != null)
                {
                    PendingRequestsSent = user.Inviters.Where(f => f.Status == FriendshipStatus.Pending).ToList();
                }
                if (user.Invitees != null)
                {
                    PendingRequestsRecieved = user.Invitees.Where(f => f.Status == FriendshipStatus.Pending).ToList();
                }
               
            }
            
            
        }
        public async Task<IActionResult> OnPostAddFriendAsync()
        {
            // optimize later (make the code above reusable) 

            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userID);
            User friend = default;

            if (userID != null)
            {
                if (!string.IsNullOrEmpty(EmailAddress))
                {
                    friend = await _userManager.FindByEmailAsync(EmailAddress);
                }
                else
                {
                    friend = await _context.Users.FirstOrDefaultAsync(m => m.NickName == NewFriendName);
                }

               

              

                if (friend != null)
                {
                  
                    Friendship newRequest = new Friendship
                    {
                        User1Id = userID,
                        User1 = user,
                        User2Id = friend.Id.ToString(),
                        User2 = friend,
                        Status = FriendshipStatus.Pending

                    };
                    user.Inviters.Add(newRequest);
                    friend.Invitees.Add(newRequest);
                    _context.Friendships.Add(newRequest);
                    await _context.SaveChangesAsync();
                    ProfilePictureUrl = user.ProfilePicturePath;
                   
          

               
                    StatusMessage = "Request has been sent";
                    return Page();
                }

            }  
            StatusMessage = "No such user exists";
            return RedirectToPage();
           
        }
    }
}
