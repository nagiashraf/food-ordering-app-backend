using FoodOrderingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; init; }

    public DbSet<Restaurant> Restaurants { get; init; }
}