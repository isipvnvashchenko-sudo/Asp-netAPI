using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationStas.DTOs;
using WebApplicationStas.Services;

namespace WebApplicationStas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _service;
    private readonly IAuthorizationService _authorizationService;

    public ReservationsController(IReservationService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
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
    [Authorize]
    public async Task<ActionResult<ReservationDto>> Release(int id)
    {
        var reservation = await _service.GetEntityAsync(id);
        if (reservation == null)
            return NotFound();

        var authorization = await _authorizationService.AuthorizeAsync(User, reservation, "SameUserOrAdmin");
        if (!authorization.Succeeded)
            return Forbid();

        var released = await _service.ReleaseAsync(id);
        return released == null ? NotFound() : Ok(released);
    }
}
