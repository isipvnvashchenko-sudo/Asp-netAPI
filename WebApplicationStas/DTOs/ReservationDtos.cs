namespace WebApplicationStas.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public int UserId { get; set; }
    public UserShortDto User { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductShortDto Product { get; set; } = null!;
}

public class CreateReservationDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
