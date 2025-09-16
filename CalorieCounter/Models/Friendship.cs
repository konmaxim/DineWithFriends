using System.ComponentModel.DataAnnotations;

namespace CalorieCounter.Models
{
    public enum FriendshipStatus
    {
        Pending,
        Accepted,
        Decline
    }
    public class Friendship
    {

        public string User1Id { get; set; }
        public User User1 { get; set; }
        public string User2Id { get; set; }
        public User User2 { get; set; }
        
        public FriendshipStatus Status { get; set; }
        public int IsFavorite = 0;
    }
}
