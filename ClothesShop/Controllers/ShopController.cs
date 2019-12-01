using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothesShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
		private readonly ClothesShopContext _context;
		public ShopController(ClothesShopContext context)
		{
			_context = context;
		}

		// GET: api/Products
		[HttpGet("Products")]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			return await _context.Product.ToListAsync();
		}

		// GET: api/Products/5
		[HttpGet("Product={id}")]
		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			var product = await _context.Product.FindAsync(id);

			if (product == null)
			{
				return NotFound();
			}

			return product;
		}

		// GET: api/CashDeskActions
		[HttpGet("Receipts")]
		public async Task<ActionResult<IEnumerable<CashDeskAction>>> GetCashDeskAction()
		{
			return await _context.CashDeskAction.ToListAsync();
		}

		// GET: api/CashDeskActions/5
		[HttpGet("Receipt={id}")]
		public async Task<ActionResult<CashDeskAction>> GetCashDeskAction(int id)
		{
			var cashDeskAction = await _context.CashDeskAction.FindAsync(id);

			if (cashDeskAction == null)
			{
				return NotFound();
			}

			return cashDeskAction;
		}

		[HttpPost("PurchaseProduct={id}")]
		public async Task<IActionResult> PurchaseAsync(int id)
		{
			var product = await _context.Product
				.Include(r=>r.CashDeskActions)
				.FirstOrDefaultAsync(r=>r.ProductId==id);

			if(product == null) {
				return NotFound();
			}

			if (product.Quantity <= 0) {
				return BadRequest();
			}

			product.Quantity--;
			if(product.CashDeskActions == null) 
				product.CashDeskActions = new List<CashDeskAction>();
			var purchaseAction = new CashDeskAction(true);
			product.CashDeskActions.Add(purchaseAction);
			_context.Entry(product).State = EntityState.Modified;

			try
			{
				_context.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				throw;
			}
			return CreatedAtAction("GetCashDeskAction", new { id = purchaseAction.ActionId }, purchaseAction);
		}

		[HttpPost("ReturnProduct={id}")]
		public async Task<IActionResult> ReturnAsync(int id)
		{
			var purchaseAction = await _context.CashDeskAction
				.Include(r => r.Product)
				.FirstOrDefaultAsync(r => r.ActionId == id);

			if (purchaseAction == null)
				return NotFound();

			string message;

			if (!purchaseAction.Product.Returnable)
				return BadRequest(new { message = "The product is not returnable" });
			
			if (DateTime.UtcNow.Subtract(purchaseAction.Date).Days > 30)
				return BadRequest(new { message = "Over 30 days, return not accepted" });

			message = DateTime.UtcNow.Subtract(purchaseAction.Date).Days < 15 
				? "Under 15 days, cash return" 
				: "Between 15 to 30 days, 'check' return";
			
			purchaseAction.Product.Quantity++;
			purchaseAction.Product.CashDeskActions.Add(new CashDeskAction(false));
			_context.Entry(purchaseAction.Product).State = EntityState.Modified;

			try
			{
				_context.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				throw;
			}
			return Accepted(new{message});

		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProductQuantityAsync(int id, Product product)
		{
			if (product == null || id <= 0 || product.Quantity < 0) {
				return BadRequest();
			}

			var productToUpdate =await _context.Product.FindAsync(id);
			productToUpdate.Quantity = product.Quantity;
			_context.Entry(productToUpdate).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(id))
				{
					return NotFound();
				}
				throw;
			}
			return NoContent();
		}
		[HttpPost]
		public async Task<ActionResult<Product>> CreateProduct(Product product)
		{
			_context.Product.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
		}
		// DELETE: api/Products/5
		[HttpDelete("Product={id}")]
		public async Task<ActionResult<Product>> DeleteProduct(int id)
		{
			var product = await _context.Product.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			_context.Product.Remove(product);
			await _context.SaveChangesAsync();

			return product;
		}

		private bool ProductExists(int id)
		{
			return _context.Product.Any(e => e.ProductId == id);
		}
	}
}
