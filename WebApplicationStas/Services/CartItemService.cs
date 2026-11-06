using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface ICartItemService
{
    Task<PagedResult<CartItemDto>> GetPagedAsync(int page, int pageSize);
    Task<CartItemDto?> GetByIdAsync(int id);
    Task<CartItemDto> CreateAsync(CreateCartItemDto dto);
    Task<CartItemDto?> UpdateAsync(int id, UpdateCartItemDto dto);
    Task<CartItemDto?> PatchAsync(int id, PatchCartItemDto dto);
    Task<bool> DeleteAsync(int id);
}

public class CartItemService : ICartItemService
{
    private readonly IRepository<CartItem> _cartItems;
    private readonly IMapper _mapper;

    public CartItemService(IRepository<CartItem> cartItems, IMapper mapper)
    {
        _cartItems = cartItems;
        _mapper = mapper;
    }

    public async Task<PagedResult<CartItemDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _cartItems.GetPagedAsync(page, pageSize, ci => ci.Id, "Product");
        var total = await _cartItems.CountAsync();
        return new PagedResult<CartItemDto>(_mapper.Map<List<CartItemDto>>(items), total);
    }

    public async Task<CartItemDto?> GetByIdAsync(int id)
    {
        var cartItem = await _cartItems.GetByIdAsync(id, "Product");
        return cartItem == null ? null : _mapper.Map<CartItemDto>(cartItem);
    }

    public async Task<CartItemDto> CreateAsync(CreateCartItemDto dto)
    {
        var cartItem = _mapper.Map<CartItem>(dto);
        _cartItems.Add(cartItem);
        await _cartItems.SaveChangesAsync();

        var created = await _cartItems.GetByIdAsync(cartItem.Id, "Product");
        return _mapper.Map<CartItemDto>(created!);
    }

    public async Task<CartItemDto?> UpdateAsync(int id, UpdateCartItemDto dto)
    {
        var cartItem = await _cartItems.GetByIdAsync(id);
        if (cartItem == null)
            return null;

        _mapper.Map(dto, cartItem);
        await _cartItems.SaveChangesAsync();

        var updated = await _cartItems.GetByIdAsync(id, "Product");
        return _mapper.Map<CartItemDto>(updated!);
    }

    public async Task<CartItemDto?> PatchAsync(int id, PatchCartItemDto dto)
    {
        var cartItem = await _cartItems.GetByIdAsync(id);
        if (cartItem == null)
            return null;

        _mapper.Map(dto, cartItem);
        await _cartItems.SaveChangesAsync();

        var updated = await _cartItems.GetByIdAsync(id, "Product");
        return _mapper.Map<CartItemDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cartItem = await _cartItems.GetByIdAsync(id);
        if (cartItem == null)
            return false;

        _cartItems.Remove(cartItem);
        await _cartItems.SaveChangesAsync();
        return true;
    }
}
