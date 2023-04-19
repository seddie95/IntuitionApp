using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntuitionApp.Data;
using IntuitionApp.Models;
using IntuitionApp.Models.Items;
using Microsoft.Data.SqlClient;

namespace IntuitionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BasketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Baskets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets()
        {
            if (_context.Baskets == null)
            {
                return NotFound();
            }
            return await _context.Baskets.ToListAsync();
        }

        // GET: api/Baskets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Basket>> GetBasket(int userId)
        {
            if (_context.Users == null)
            {
                return NotFound("No users not found");
            }

            var user = await _context.Users
                .Include(u => u.Basket)
                .FirstOrDefaultAsync(us => us.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var basket = user.Basket;
            if (basket == null)
            {
                return NotFound("Basket not found");
            }

            return basket;
        }



        // POST: api/Baskets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<Basket>> UpdateToBasket(List<BasketItem> items, int userId)
        {
            if (_context.Baskets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Baskets'  is null.");
            }

            if (items == null || items.Count == 0)
            {
                return BadRequest();
            }

            var basket = await _context.Baskets
                .Include(x => x.BasketItems)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (basket == null)
            {
                return NotFound("No Basket found for user: " + userId);
            }

            // loop through the order items and check that the orderId matches
            List<BasketItem> matchingItems = items.Where(oi => oi.BasketId == basket.Id).ToList();

            if (matchingItems.Count == 0)
            {
                return NotFound("No basket items found for user: " + userId);
            }

            basket.BasketItems = matchingItems;

            _context.Entry(basket).State = EntityState.Modified;

            try
            {
                // Save the changes in the db
                await _context.SaveChangesAsync();

                // Update the total amount in the orders table
                //_context.Database.ExecuteSqlRaw("EXECUTE UpdateOrderTotalAmount @OrderId", new SqlParameter("@OrderId", id));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BasketExists(basket.Id))
                {
                    return NotFound("Basket item not found in database");
                }
                throw;
            }

            return NoContent();

        }

        private bool BasketExists(int id)
        {
            return (_context.Baskets?.Any(e => e.Id == id)).GetValueOrDefault();
        }



    }
}
