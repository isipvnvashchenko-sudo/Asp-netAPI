using Microsoft.AspNetCore.Mvc;
using WebApplicationStas.DTOs;
using WebApplicationStas.Services;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _service;

    public CartItemsController(ICartItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result.Items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartItemDto>> GetById(int id)
    {
        var cartItem = await _service.GetByIdAsync(id);
        return cartItem == null ? NotFound() : Ok(cartItem);
    }

    [HttpPost]
    public async Task<ActionResult<CartItemDto>> Create([FromBody] CreateCartItemDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCartItemDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchCartItemDto dto)
    {
        var patched = await _service.PatchAsync(id, dto);
        return patched == null ? NotFound() : NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
