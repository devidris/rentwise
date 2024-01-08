using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentWise.Models;
using RentWise.Models.Identity;
using System.Reflection.Emit;

namespace RentWise.DataAccess;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<AgentRegistrationModel> AgentRegistrations { get; set; }

    public DbSet<ProductModel> Products { get; set; }

    public DbSet<ReviewModel> Reviews { get; set; }

    public DbSet<LikeModel> Likes { get; set; }

    public DbSet<ChatModel> Chats { get; set; }

    public DbSet<OrdersModel> Orders { get; set; }

    public DbSet<UsersDetailsModel> UsersDetails { get; set; }
    public DbSet<ContactAdminModel> ContactAdmin { get; set; }

    public DbSet<WithdrawalHistoryModel> WithdrawalHistories { get; set; }
    public DbSet<ProductImageModel> ProductImages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AgentRegistrationModel>()
     .HasIndex(u => u.PhoneNumber)
 .IsUnique();

        modelBuilder.Entity<AgentRegistrationModel>()
            .HasIndex(u => u.StoreName)
        .IsUnique();

        modelBuilder.Entity<AgentRegistrationModel>()
            .HasIndex(u => u.Slug)
            .IsUnique();
        
    }
}
