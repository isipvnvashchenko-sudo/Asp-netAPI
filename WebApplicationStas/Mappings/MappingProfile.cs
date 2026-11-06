using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;

namespace WebApplicationStas.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryShortDto>();
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products));

        CreateMap<Product, ProductShortDto>();
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category));

        CreateMap<User, UserShortDto>();
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role));

        CreateMap<Role, RoleShortDto>();
        CreateMap<Role, RoleDto>()
            .ForMember(d => d.Users, o => o.MapFrom(s => s.Users));

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.User, o => o.MapFrom(s => s.User))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.CartItems));

        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.User, o => o.MapFrom(s => s.User))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product));

        CreateMap<ProductReservation, ReservationDto>()
            .ForMember(d => d.User, o => o.MapFrom(s => s.User))
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<PatchCategoryDto, Category>()
            .ForMember(d => d.Name, o => o.Condition(s => s.Name != null))
            .ForMember(d => d.Description, o => o.Condition(s => s.Description != null));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<PatchProductDto, Product>()
            .ForMember(d => d.Name, o => o.Condition(s => s.Name != null))
            .ForMember(d => d.Description, o => o.Condition(s => s.Description != null))
            .ForMember(d => d.Price, o => o.Condition(s => s.Price.HasValue))
            .ForMember(d => d.ImageUrl, o => o.Condition(s => s.ImageUrl != null))
            .ForMember(d => d.StockQuantity, o => o.Condition(s => s.StockQuantity.HasValue))
            .ForMember(d => d.CategoryId, o => o.Condition(s => s.CategoryId.HasValue));

        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<PatchUserDto, User>()
            .ForMember(d => d.Username, o => o.Condition(s => s.Username != null))
            .ForMember(d => d.Password, o => o.Condition(s => s.Password != null))
            .ForMember(d => d.Email, o => o.Condition(s => s.Email != null))
            .ForMember(d => d.IsActive, o => o.Condition(s => s.IsActive.HasValue))
            .ForMember(d => d.RoleId, o => o.Condition(s => s.RoleId.HasValue));

        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>();
        CreateMap<PatchRoleDto, Role>()
            .ForMember(d => d.Name, o => o.Condition(s => s.Name != null));

        CreateMap<CreateCartDto, Cart>()
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateCartDto, Cart>();
        CreateMap<PatchCartDto, Cart>()
            .ForMember(d => d.UserId, o => o.Condition(s => s.UserId.HasValue))
            .ForMember(d => d.IsActive, o => o.Condition(s => s.IsActive.HasValue));

        CreateMap<CreateCartItemDto, CartItem>();
        CreateMap<UpdateCartItemDto, CartItem>();
        CreateMap<PatchCartItemDto, CartItem>()
            .ForMember(d => d.CartId, o => o.Condition(s => s.CartId.HasValue))
            .ForMember(d => d.ProductId, o => o.Condition(s => s.ProductId.HasValue))
            .ForMember(d => d.Quantity, o => o.Condition(s => s.Quantity.HasValue));

        CreateMap<CreateOrderDto, Order>()
            .ForMember(d => d.OrderDate, o => o.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateOrderDto, Order>();
        CreateMap<PatchOrderDto, Order>()
            .ForMember(d => d.UserId, o => o.Condition(s => s.UserId.HasValue))
            .ForMember(d => d.TotalAmount, o => o.Condition(s => s.TotalAmount.HasValue))
            .ForMember(d => d.Status, o => o.Condition(s => s.Status != null));

        CreateMap<CreateOrderItemDto, OrderItem>();
        CreateMap<UpdateOrderItemDto, OrderItem>();
        CreateMap<PatchOrderItemDto, OrderItem>()
            .ForMember(d => d.OrderId, o => o.Condition(s => s.OrderId.HasValue))
            .ForMember(d => d.ProductId, o => o.Condition(s => s.ProductId.HasValue))
            .ForMember(d => d.Quantity, o => o.Condition(s => s.Quantity.HasValue))
            .ForMember(d => d.UnitPrice, o => o.Condition(s => s.UnitPrice.HasValue));

        CreateMap<CreateReservationDto, ProductReservation>()
            .ForMember(d => d.ReservedAt, o => o.MapFrom(_ => DateTime.UtcNow));
    }
}
