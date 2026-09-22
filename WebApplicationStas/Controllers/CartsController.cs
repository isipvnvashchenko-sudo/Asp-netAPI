using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationStas.DTOs;
using WebApplicationStas.Mappings;
using WebApplicationStas.Models;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ShopdbContext _context;

    public CartsController(ShopdbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var carts = await _context.Carts
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.ToDto())
            .ToListAsync();

        return Ok(carts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartDto>> GetById(int id)
    {
        var cart = await _context.Carts.FindAsync(id);

        if (cart == null)
            return NotFound();

        return Ok(cart.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CartDto>> Create([FromBody] CreateCartDto dto)
    {
        var cart = dto.ToEntity();

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        var resultDto = cart.ToDto();

        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCartDto dto)
    {
        var cart = await _context.Carts.FindAsync(id);

        if (cart == null)
            return NotFound();

        cart.UserId = dto.UserId;
        cart.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchCartDto dto)
    {
        var cart = await _context.Carts.FindAsync(id);

        if (cart == null)
            return NotFound();

        if (dto.UserId.HasValue) cart.UserId = dto.UserId.Value;
        if (dto.IsActive.HasValue) cart.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cart = await _context.Carts.FindAsync(id);

        if (cart == null)
            return NotFound();

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}