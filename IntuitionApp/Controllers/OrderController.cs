using IntuitionApp.Data;
using IntuitionApp.Models;
using IntuitionApp.Models.Items;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntuitionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a specific order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Orders/5

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound("No orders found.");
            }

            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("No orders found for id:" + id);
            }

            return order;
        }

        /// <summary>
        ///  Retrieves a paginated list of all orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Orders/5

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders(int page = 1, int pageSize = 10)
        {
            if (_context.Orders == null)
            {
                return NotFound("No orders found.");
            }

            if (_context.OrderItems == null)
            {
                return NotFound("No order items found.");
            }

            var orders = await _context.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.OrderItems)
                .ToListAsync();


            if (orders == null)
            {
                return NotFound("No orders found");
            }

            return orders;
        }

        /// <summary>
        /// Creates a new Order
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(List<Item> items, string address, int userId)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            if (items == null || items.Count == 0)
            {
                return BadRequest();
            }

            // Create a new order 
            Order order = new Order();
            order.UserId = userId;
            order.Address = address;

            decimal totalAmount = 0;

            foreach (Item item in items)
            {
                if (item.Quantity > 0 && item.ProductId > 0)
                {
                    OrderItem orderItem = new OrderItem(item);
                    totalAmount += item.Price;
                    order.OrderItems.Add(orderItem);
                }
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Orders OFF");

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        /// <summary>
        /// Updates the order items for a given order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderItem"></param>
        /// <returns></returns>
        [HttpPut("{id}/orderItems")]
        public async Task<IActionResult> UpdateOrderItems(int id, List<OrderItem> orderItems)
        {
            if (orderItems == null)
            {
                return BadRequest("Order items passed is null");
            }

            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OrderItems'  is null.");
            }

            var order = await _context.Orders
               .Include(x => x.OrderItems)
               .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("No orders found for id:" + id);
            }

            // loop through the order items and check that the orderId matches
            List<OrderItem> matchingItems = orderItems.Where(oi => oi.OrderId == id).ToList();

            if (matchingItems.Count == 0)
            {
                return NotFound("No order items found for id:" + id);
            }

            order.OrderItems = matchingItems;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                // Save the changes in the db
                await _context.SaveChangesAsync();

                // Update the total amount in the orders table
                _context.Database.ExecuteSqlRaw("EXECUTE UpdateOrderTotalAmount @OrderId", new SqlParameter("@OrderId", id));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound("Order item not found in database");
                }
                throw;
            }

            return NoContent();

        }

        /// <summary>
        /// Updates the address of the order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpPut("{id}/address")]
        public async Task<IActionResult> UpdateOrderAddress(int id, string address)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("No order found");
            }

            order.Address = address;

            return await UpdateOrder(id, order);

        }

        /// <summary>
        /// Sets the order status to cancelled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("No order found");
            }

            order.OrderStatus = OrderStatus.Canceled;

            return await UpdateOrder(id, order);

        }

        /// <summary>
        /// Updates the order in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<IActionResult> UpdateOrder(int id, Order? order)
        {
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                // Save the changes in the db
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound("Order item not found in database");
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Checks if an order exists in the context
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Checks if an order item exists in the context
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool OrderItemExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
