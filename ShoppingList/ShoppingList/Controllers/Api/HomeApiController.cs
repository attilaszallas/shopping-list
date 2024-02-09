using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/api/shoppingItems")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetShoppingItemsAsync()
        {
            return new JsonResult(await _context.ShoppingItems.ToListAsync());
        }

        [HttpGet]
        [Route("/api/shoppingItem/{id}")]
        public async Task<IActionResult> GetShoppingItemByIndexAsync(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var shoppingItem = await _context.ShoppingItems.FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingItem == null)
            {
                return NotFound();
            }

            return new JsonResult(shoppingItem);
        }

        [HttpPost]
        [Route("/api/shoppingItem/{id}")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsPurchased")] ShoppingItem shoppingItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingItem);
                await _context.SaveChangesAsync();
                return new JsonResult(shoppingItem);
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("/api/shoppingItem/{id}")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsPurchased")] ShoppingItem shoppingItem)
        {
            if (id != shoppingItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingItemExists(shoppingItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return new JsonResult(shoppingItem);
            }

            return BadRequest();
        }

        [HttpDelete, ActionName("Delete")]
        [Route("/api/shoppingItem/{id}")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shoppingItem = await _context.ShoppingItems.FindAsync(id);
            if (shoppingItem != null)
            {
                _context.ShoppingItems.Remove(shoppingItem);
                await _context.SaveChangesAsync();

                return new JsonResult(shoppingItem);
            }

            return BadRequest();
        }

        private bool ShoppingItemExists(int id)
        {
            return _context.ShoppingItems.Any(e => e.Id == id);
        }
    }
}
