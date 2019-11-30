using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothesShop.Models;

namespace ClothesShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashDeskActionsController : ControllerBase
    {
        private readonly ClothesShopContext _context;

        public CashDeskActionsController(ClothesShopContext context)
        {
            _context = context;
        }

        // GET: api/CashDeskActions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CashDeskAction>>> GetCashDeskAction()
        {
            return await _context.CashDeskAction.ToListAsync();
        }

        // GET: api/CashDeskActions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CashDeskAction>> GetCashDeskAction(int id)
        {
            var cashDeskAction = await _context.CashDeskAction.FindAsync(id);

            if (cashDeskAction == null)
            {
                return NotFound();
            }

            return cashDeskAction;
        }

        // PUT: api/CashDeskActions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCashDeskAction(int id, CashDeskAction cashDeskAction)
        {
            if (id != cashDeskAction.ActionId)
            {
                return BadRequest();
            }

            _context.Entry(cashDeskAction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CashDeskActionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CashDeskActions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CashDeskAction>> PostCashDeskAction(CashDeskAction cashDeskAction)
        {
            _context.CashDeskAction.Add(cashDeskAction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCashDeskAction", new { id = cashDeskAction.ActionId }, cashDeskAction);
        }

        // DELETE: api/CashDeskActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CashDeskAction>> DeleteCashDeskAction(int id)
        {
            var cashDeskAction = await _context.CashDeskAction.FindAsync(id);
            if (cashDeskAction == null)
            {
                return NotFound();
            }

            _context.CashDeskAction.Remove(cashDeskAction);
            await _context.SaveChangesAsync();

            return cashDeskAction;
        }

        private bool CashDeskActionExists(int id)
        {
            return _context.CashDeskAction.Any(e => e.ActionId == id);
        }
    }
}
