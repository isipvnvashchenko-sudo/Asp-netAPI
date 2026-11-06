namespace WebApplicationStas.DTOs;

public class CartItemDto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductShortDto Product { get; set; } = null!;
}

public class CreateCartItemDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class PatchCartItemDto
{
    public int? CartId { get; set; }
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}
