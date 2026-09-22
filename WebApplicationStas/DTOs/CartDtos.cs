namespace WebApplicationStas.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCartDto
{
    public int UserId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCartDto
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}

public class PatchCartDto
{
    public int? UserId { get; set; }
    public bool? IsActive { get; set; }
}