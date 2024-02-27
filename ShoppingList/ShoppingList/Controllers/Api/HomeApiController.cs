using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly ILogger<HomeApiController> _logger;
        private readonly ApplicationDbContext _context;

        private IQueryable<ShoppingItem> ShoppingItemsFilteredByUser => _context.ShoppingItems.Where(user => user.UserEmail == LoggedUserName);

        private string LoggedUserName => GetLoggedUserName();

        public HomeApiController(ILogger<HomeApiController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("/api/shoppingItems")]
        public async Task<IActionResult> GetShoppingItemsAsync()
        {
            return new JsonResult(await ShoppingItemsFilteredByUser.ToListAsync());
        }

        [HttpGet]
        [Route("/api/shoppingItem/{id}")]
        public async Task<IActionResult> GetShoppingItemByIndexAsync(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var shoppingItem = await ShoppingItemsFilteredByUser.FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingItem == null)
            {
                return NotFound();
            }

            return new JsonResult(shoppingItem);
        }

        [HttpPost]
        [Route("/api/shoppingItem/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsInTheShoppingCart")] ShoppingItem shoppingItem)
        {
            if (shoppingItem is not null)
            {
                _context.Add(shoppingItem);
                await _context.SaveChangesAsync();
                return new JsonResult(shoppingItem);
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("/api/shoppingItem/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsInTheShoppingCart")] ShoppingItem shoppingItem)
        {
            if (id != shoppingItem.Id)
            {
                return NotFound();
            }

            if (shoppingItem is not null)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shoppingItem = await ShoppingItemsFilteredByUser.FirstAsync(user => user.Id == id);
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
            return ShoppingItemsFilteredByUser.Any(e => e.Id == id);
        }

        private string GetLoggedUserName()
        {
            var identityName = HttpContext.User?.Identity?.Name;

            if (identityName is null or "")
            {
                throw new InvalidOperationException("User cannot identified.");
            }

            return identityName;
        }
    }
}
