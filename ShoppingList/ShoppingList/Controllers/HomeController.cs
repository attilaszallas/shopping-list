using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;
using System.Diagnostics;

namespace ShoppingList.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        private IQueryable<ShoppingItem> ShoppingItemsFilteredByUser => _context.ShoppingItems.Where(user => user.UserEmail == LoggedUserName);

        private string LoggedUserName => GetLoggedUserName();

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _shoppingItems = await ShoppingItemsFilteredByUser.ToListAsync();

            var _viewModel = new ShoppingItemViewModel();
            _viewModel.ShoppingItems = _shoppingItems;

            return View(_viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ShoppingItemViewModel shoppingItemViewModel)
        {
            if (shoppingItemViewModel is not null)
            {
                var shoppingItem = shoppingItemViewModel.NewShoppingItem;

                if (shoppingItem is null)
                {
                    return BadRequest();
                }

                if (shoppingItem.Name is null or "")
                {
                    return BadRequest("Shopping item name is null or empty.");
                }

                shoppingItem.UserEmail = LoggedUserName;

                _context.Add(shoppingItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

        // GET: ShoppingItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingItem = await ShoppingItemsFilteredByUser.FirstAsync(user => user.Id == id);
            if (shoppingItem == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ShoppingItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsInTheShoppingCart")] ShoppingItem shoppingItem)
        {
            if (id != shoppingItem.Id)
            {
                return NotFound();
            }

            shoppingItem.UserEmail = LoggedUserName;

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

                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shoppingItem = await ShoppingItemsFilteredByUser.FirstAsync(user => user.Id == id);
            if (shoppingItem != null)
            {
                _context.ShoppingItems.Remove(shoppingItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmptyList()
        {
            _context.ShoppingItems.RemoveRange(ShoppingItemsFilteredByUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchIsPurchased(int id)
        {
            var shoppingItem = await ShoppingItemsFilteredByUser.FirstAsync(user => user.Id == id);
            if (shoppingItem != null)
            {
                shoppingItem.IsInTheShoppingCart = !shoppingItem.IsInTheShoppingCart;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingItemExists(int id)
        {
            return _context.ShoppingItems.Any(e => e.Id == id);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
