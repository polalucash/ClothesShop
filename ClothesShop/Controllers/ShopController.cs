using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothesShop.Data;
using ClothesShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothesShop.Controllers
{
    [Route("[api/controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
		private readonly ClothesShopContext _context;
		public ShopController(ClothesShopContext context)
		{
			_context = context;
		}
		

		// GET: shop/Products
		[HttpGet("Products")]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			return await _context.Product
				.AsNoTracking()
				.ToListAsync();
		}

		// GET: shop/Products/5
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

		// GET: shop/Purchases
		[HttpGet("Purchases")]
		public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase()
		{
			return await _context.Purchase.ToListAsync();
		}

		// GET: shop/Purchases/5
		[HttpGet("Purchases={id}")]
		public async Task<ActionResult<Purchase>> GetPurchase(int id)
		{
			var purchase = await _context.Purchase.FindAsync(id);

			if (purchase == null)
			{
				return NotFound();
			}

			return purchase;
		}
		// GET: shop/statistics?year=2019&month=12
		[HttpGet("statistics")]
		public ActionResult<IEnumerator<MonthlyStatisticRecord>> GetMonthlyStatistics(int year, int month)
		{
			if (year < 1 || month < 1 || month > 12 || year > DateTime.Today.Year)
			{
				return BadRequest();
			}

			var dailyPurchases = _context.Purchase
				.Where(r => r.PurchaseDate.Month == month
							&& r.PurchaseDate.Year == year)
				.AsNoTracking()
				.ToList()
				.GroupBy(r => r.PurchaseDate.Day)
				.ToDictionary(r => r.Key, r => r.Count());

			var dailyReturns = _context.Purchase
				.Where(r => r.ReturnDate.HasValue
							&& r.ReturnDate.Value.Date.Month == month
							&& r.ReturnDate.Value.Date.Year == year)
				.AsNoTracking()
				.ToList()
				.GroupBy(r => r.ReturnDate.Value.Day)
				.ToDictionary(r => r.Key, r => r.Count());

			return new JsonResult(Enumerable.Range(1, DateTime.DaysInMonth(year, month))
				.Select(i => new MonthlyStatisticRecord
				{
					Day = i,
					Returns = dailyReturns.TryGetValue(i, out var returns) ? returns : 0,
					Purchases = dailyPurchases.TryGetValue(i, out var purchases) ? purchases : 0
				}).GetEnumerator());
		}

		[HttpPost("PurchaseProduct")]
		public async Task<IActionResult> PurchaseProductAsync(int id)
		{
			var product = await _context.Product
				.Include(r=>r.Purchases)
				.FirstOrDefaultAsync(r=>r.ProductId==id);

			if(product == null) {
				return NotFound();
			}

			if (product.Quantity <= 0) {
				return BadRequest();
			}

			var purchase = product.Purchase();
			_context.Entry(product).State = EntityState.Modified;

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
			return CreatedAtAction("GetPurchase", new { id = purchase.PurchaseId }, purchase);
		}

		[HttpPut("ReturnPurchase")]
		public async Task<IActionResult> ReturnPurchaseAsync(int id)
		{
			var purchase = await _context.Purchase
				.Include(r => r.Product)
				.FirstOrDefaultAsync(r => r.PurchaseId == id);

			if (purchase == null)
				return NotFound();

			if (!purchase.Returnable || !purchase.Product.Returnable)
				return BadRequest(new { message = "The product is not returnable" });
			
			if (DateTime.UtcNow.Subtract(purchase.PurchaseDate).Days > 30)
				return BadRequest(new { message = "Over 30 days, return not accepted" });

			purchase.Return();
			_context.Entry(purchase).State = EntityState.Modified;
			_context.Entry(purchase.Product).State = EntityState.Modified;

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

			var message = DateTime.UtcNow.Subtract(purchase.PurchaseDate).Days < 15
				? "Under 15 days, cash return"
				: "Between 15 to 30 days, 'check' return";

			return Accepted(new{ message, purchase });
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProductQuantityAsync(int id, Product product)
		{
			if (product == null || id <= 0 || product.Quantity < 0) {
				return BadRequest();
			}

			var productToUpdate = await _context.Product
				.AsTracking()
				.FirstOrDefaultAsync(r => r.ProductId == id);

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
		[HttpPost("Product")]
		public async Task<ActionResult<Product>> CreateProduct(Product product)
		{
			_context.Product.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
		}
		private bool ProductExists(int id)
		{
			return _context.Product.Any(e => e.ProductId == id);
		}
	}
}
