namespace WebApplicationStas.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserShortDto User { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class CreateOrderDto
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
}

public class UpdateOrderDto
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PatchOrderDto
{
    public int? UserId { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Status { get; set; }
}
