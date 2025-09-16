using CalorieCounter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace CalorieCounter.Data
{
    public class DishDbContext: IdentityDbContext<User>
    {
        public DishDbContext(DbContextOptions<DishDbContext> options) : base(options)
        {

        }
       
        public DbSet<Dish> Dishes { get; set; } = default!;
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<DishImage> DishImages { get; set; } = default!;
        public DbSet<Ingredient> Ingredients { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Friendship>()
                .HasKey(fs => new { fs.User1Id, fs.User2Id});
            //each user has many inviters, and the second user then has many invitees
           //many-to-many relationship with friendship class as the join
            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.User1)
                .WithMany(u => u.Inviters)
                .HasForeignKey(fs => fs.User1Id)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.User2)
                .WithMany(u => u.Invitees)
                .HasForeignKey(fs => fs.User2Id)
                .OnDelete(DeleteBehavior.Restrict);
        
        }
    }
}
