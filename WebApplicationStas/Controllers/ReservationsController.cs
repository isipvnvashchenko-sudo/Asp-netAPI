using Microsoft.AspNetCore.Mvc;
using WebApplicationStas.DTOs;
using WebApplicationStas.Services;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _service;

    public ReservationsController(IReservationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result.Items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var reservation = await _service.GetByIdAsync(id);
        return reservation == null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Reserve([FromBody] CreateReservationDto dto)
    {
        var created = await _service.ReserveAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/release")]
    public async Task<ActionResult<ReservationDto>> Release(int id)
    {
        var released = await _service.ReleaseAsync(id);
        return released == null ? NotFound() : Ok(released);
    }
}
