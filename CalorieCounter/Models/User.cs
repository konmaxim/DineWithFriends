using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace CalorieCounter.Models
{
    public class User : IdentityUser
    {
        public string NickName { get; set; } = "";
       
        public DateOnly BirthDay { get; set; }
        public string? ProfilePicturePath { get; set; }
        public virtual List<Friendship>? Inviters { get; set; } = new List<Friendship>();
      
        public virtual List<Friendship>? Invitees { get; set; } = new List<Friendship>();
    }
}
