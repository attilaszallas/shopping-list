using Microsoft.EntityFrameworkCore;
using ShoppingList.Models;

namespace ShoppingList.Data
{
    public class ShoppingListDbContext: DbContext
    {
        public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options): base(options)
        {
        }

        public DbSet<ShoppingItem> ShoppingItems { get; set; }
    }
}
