using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_DSCS2_WEBBANGIAY.Models;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public SizesController(ShoesEcommereContext context)
        {
            _context = context;
        }

        // GET: api/Sizes
        [HttpGet]
        public async Task<ActionResult> GetSizes()
        {
            var sizes = await _context.Sizes.ToListAsync();
            var select = sizes.Select(x => new
            {
                label = x.Size1,
                value = x.Id,
            });
            return Ok(select);
        }

        //// GET: api/Sizes/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Size>> GetSize(int id)
        //{
        //    var size = await _context.Sizes.FindAsync(id);

        //    if (size == null)
        //    {
        //        return NotFound();
        //    }

        //    return size;
        //}

        // PUT: api/Sizes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutSize(string id, Size size)
        //{
        //    if (id != size.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(size).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SizeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Sizes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Size>> PostSize(Size size)
        //{
        //    _context.Sizes.Add(size);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetSize", new { id = size.Id }, size);
        //}

        // DELETE: api/Sizes/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSize(int id)
        //{
        //    var size = await _context.Sizes.FindAsync(id);
        //    if (size == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Sizes.Remove(size);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool SizeExists(string id)
        //{
        //    return _context.Sizes.Any(e => e.Id == id);
        //}
    }
}
