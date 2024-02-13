namespace ShoppingList.Models
{
    public class ShoppingItemViewModel
    {
        public ShoppingItem NewShoppingItem { get; set; }
        public IEnumerable<ShoppingItem> ShoppingItems { get; set;}
    }
}
