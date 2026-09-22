using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationStas.DTOs;
using WebApplicationStas.Mappings;
using WebApplicationStas.Models;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly ShopdbContext _context;

    public OrderItemsController(ShopdbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var orderItems = await _context.OrderItems
            .OrderBy(oi => oi.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(oi => oi.ToDto())
            .ToListAsync();

        return Ok(orderItems);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderItemDto>> GetById(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound();

        return Ok(orderItem.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto dto)
    {
        var orderItem = dto.ToEntity();

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        var resultDto = orderItem.ToDto();

        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderItemDto dto)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound();

        orderItem.OrderId = dto.OrderId;
        orderItem.ProductId = dto.ProductId;
        orderItem.Quantity = dto.Quantity;
        orderItem.UnitPrice = dto.UnitPrice;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchOrderItemDto dto)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound();

        if (dto.OrderId.HasValue) orderItem.OrderId = dto.OrderId.Value;
        if (dto.ProductId.HasValue) orderItem.ProductId = dto.ProductId.Value;
        if (dto.Quantity.HasValue) orderItem.Quantity = dto.Quantity.Value;
        if (dto.UnitPrice.HasValue) orderItem.UnitPrice = dto.UnitPrice.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
            return NotFound();

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}