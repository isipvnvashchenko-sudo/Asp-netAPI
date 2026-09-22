using WebApplicationStas.DTOs;
using WebApplicationStas.Models;

namespace WebApplicationStas.Mappings;

public static class MappingExtensions
{
    
    public static ProductDto ToDto(this Product entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Price = entity.Price,
        CategoryId = entity.CategoryId,
        ImageUrl = entity.ImageUrl
    };
    public static Product ToEntity(this CreateProductDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        CategoryId = dto.CategoryId,
        ImageUrl = dto.ImageUrl
    };

   
    public static RoleDto ToDto(this Role entity) => new() { Id = entity.Id, Name = entity.Name };
    public static Role ToEntity(this CreateRoleDto dto) => new() { Name = dto.Name };

    
    public static UserDto ToDto(this User entity) => new()
    {
        Id = entity.Id,
        Username = entity.Username,
        Email = entity.Email,
        IsActive = entity.IsActive,
        RoleId = entity.RoleId
    };
    public static User ToEntity(this CreateUserDto dto) => new()
    {
        Username = dto.Username,
        Password = dto.Password,
        Email = dto.Email,
        IsActive = dto.IsActive,
        RoleId = dto.RoleId
    };

    
    public static CategoryDto ToDto(this Category entity) => new() { Id = entity.Id, Name = entity.Name, Description = entity.Description };
    public static Category ToEntity(this CreateCategoryDto dto) => new() { Name = dto.Name, Description = dto.Description };

    
    public static CartDto ToDto(this Cart entity) => new() { Id = entity.Id, UserId = entity.UserId, CreatedAt = entity.CreatedAt, IsActive = entity.IsActive };
    public static Cart ToEntity(this CreateCartDto dto) => new() { UserId = dto.UserId, CreatedAt = DateTime.UtcNow, IsActive = dto.IsActive };

    
    public static CartItemDto ToDto(this CartItem entity) => new() { Id = entity.Id, CartId = entity.CartId, ProductId = entity.ProductId, Quantity = entity.Quantity };
    public static CartItem ToEntity(this CreateCartItemDto dto) => new() { CartId = dto.CartId, ProductId = dto.ProductId, Quantity = dto.Quantity };

    
    public static OrderDto ToDto(this Order entity) => new() { Id = entity.Id, UserId = entity.UserId, OrderDate = entity.OrderDate, TotalAmount = entity.TotalAmount, Status = entity.Status };
    public static Order ToEntity(this CreateOrderDto dto) => new() { UserId = dto.UserId, OrderDate = DateTime.UtcNow, TotalAmount = dto.TotalAmount, Status = dto.Status };
    

    public static OrderItemDto ToDto(this OrderItem entity) => new() { Id = entity.Id, OrderId = entity.OrderId, ProductId = entity.ProductId, Quantity = entity.Quantity, UnitPrice = entity.UnitPrice };
    public static OrderItem ToEntity(this CreateOrderItemDto dto) => new() { OrderId = dto.OrderId, ProductId = dto.ProductId, Quantity = dto.Quantity, UnitPrice = dto.UnitPrice };
}