using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppCupones.Data;
using AppCupones.Models;

namespace AppCupones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCuponController : ControllerBase
    {
        private readonly DbAppContext _context;

        public TipoCuponController(DbAppContext context)
        {
            _context = context;
        }

        // GET: api/TipoCupon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoCuponModel>>> GetTipo_Cupon()
        {
            return await _context.Tipo_Cupon.ToListAsync();
        }

        // GET: api/TipoCupon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoCuponModel>> GetTipoCuponModel(int id)
        {
            var tipoCuponModel = await _context.Tipo_Cupon.FindAsync(id);

            if (tipoCuponModel == null)
            {
                return NotFound();
            }

            return tipoCuponModel;
        }

        // PUT: api/TipoCupon/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoCuponModel(int id, TipoCuponModel tipoCuponModel)
        {
            if (id != tipoCuponModel.Id_Tipo_Cupon)
            {
                return BadRequest();
            }

            _context.Entry(tipoCuponModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoCuponModelExists(id))
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

        // POST: api/TipoCupon
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoCuponModel>> PostTipoCuponModel(TipoCuponModel tipoCuponModel)
        {
            _context.Tipo_Cupon.Add(tipoCuponModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoCuponModel", new { id = tipoCuponModel.Id_Tipo_Cupon }, tipoCuponModel);
        }

        // DELETE: api/TipoCupon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoCuponModel(int id)
        {
            var tipoCuponModel = await _context.Tipo_Cupon.FindAsync(id);
            if (tipoCuponModel == null)
            {
                return NotFound();
            }

            _context.Tipo_Cupon.Remove(tipoCuponModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoCuponModelExists(int id)
        {
            return _context.Tipo_Cupon.Any(e => e.Id_Tipo_Cupon == id);
        }
    }
}
