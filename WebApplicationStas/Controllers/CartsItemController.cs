using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationStas.DTOs;
using WebApplicationStas.Mappings;
using WebApplicationStas.Models;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemsController : ControllerBase
{
    private readonly ShopdbContext _context;

    public CartItemsController(ShopdbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var cartItems = await _context.CartItems
            .OrderBy(ci => ci.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ci => ci.ToDto())
            .ToListAsync();

        return Ok(cartItems);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartItemDto>> GetById(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
            return NotFound();

        return Ok(cartItem.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CartItemDto>> Create([FromBody] CreateCartItemDto dto)
    {
        var cartItem = dto.ToEntity();

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        var resultDto = cartItem.ToDto();

        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCartItemDto dto)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
            return NotFound();

        cartItem.CartId = dto.CartId;
        cartItem.ProductId = dto.ProductId;
        cartItem.Quantity = dto.Quantity;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchCartItemDto dto)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
            return NotFound();

        if (dto.CartId.HasValue) cartItem.CartId = dto.CartId.Value;
        if (dto.ProductId.HasValue) cartItem.ProductId = dto.ProductId.Value;
        if (dto.Quantity.HasValue) cartItem.Quantity = dto.Quantity.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
            return NotFound();

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}