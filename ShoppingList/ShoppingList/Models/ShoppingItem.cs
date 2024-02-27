using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models
{
    public class ShoppingItem
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        [Required]
        public required string Name { get; set; }
        public bool IsInTheShoppingCart { get; set; }
    }
}
