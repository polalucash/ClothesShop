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

		[HttpPost("Purchase={id}")]
		public IActionResult Purchase(int id)
		{
			var product = _context.Product.Find(id);
			if(product == null) {
				return NotFound();
			}

			if (product.Quantity <= 0) {
				return BadRequest();
			}

			product.Quantity--;
			if(product.CashDeskActions == null) 
				product.CashDeskActions = new List<CashDeskAction>();
			product.CashDeskActions.Add(new CashDeskAction(true));
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
			return NoContent();
		}

		[HttpPost("Return={id}")]
		public IActionResult Return(int id)
		{
			var product = _context.Product.Find(id);
			if (product == null)
			{
				return NotFound();
			}

			if(product.Returnable)
			{
				product.Quantity++;
				if (product.CashDeskActions == null)
					product.CashDeskActions = new List<CashDeskAction>();

				product.CashDeskActions.Add(new CashDeskAction(false));
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
				return NoContent();
			}

			return BadRequest();
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
		// GET: api/Products
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			return await _context.Product.ToListAsync();
		}

		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			var product = await _context.Product.FindAsync(id);

			if (product == null)
			{
				return NotFound();
			}

			return product;
		}
		[HttpPost]
		public async Task<ActionResult<Product>> CreateProduct(Product product)
		{
			_context.Product.Add(product);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
		}

		// DELETE: api/Products/5
		[HttpDelete("{id}")]
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
