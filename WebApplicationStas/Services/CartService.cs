using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface ICartService
{
    Task<PagedResult<CartDto>> GetPagedAsync(int page, int pageSize);
    Task<CartDto?> GetByIdAsync(int id);
    Task<CartDto> CreateAsync(CreateCartDto dto);
    Task<CartDto?> UpdateAsync(int id, UpdateCartDto dto);
    Task<CartDto?> PatchAsync(int id, PatchCartDto dto);
    Task<bool> DeleteAsync(int id);
}

public class CartService : ICartService
{
    private readonly IRepository<Cart> _carts;
    private readonly IMapper _mapper;

    public CartService(IRepository<Cart> carts, IMapper mapper)
    {
        _carts = carts;
        _mapper = mapper;
    }

    public async Task<PagedResult<CartDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _carts.GetPagedAsync(page, pageSize, c => c.Id, "User", "CartItems.Product");
        var total = await _carts.CountAsync();
        return new PagedResult<CartDto>(_mapper.Map<List<CartDto>>(items), total);
    }

    public async Task<CartDto?> GetByIdAsync(int id)
    {
        var cart = await _carts.GetByIdAsync(id, "User", "CartItems.Product");
        return cart == null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> CreateAsync(CreateCartDto dto)
    {
        var cart = _mapper.Map<Cart>(dto);
        _carts.Add(cart);
        await _carts.SaveChangesAsync();

        var created = await _carts.GetByIdAsync(cart.Id, "User", "CartItems.Product");
        return _mapper.Map<CartDto>(created!);
    }

    public async Task<CartDto?> UpdateAsync(int id, UpdateCartDto dto)
    {
        var cart = await _carts.GetByIdAsync(id);
        if (cart == null)
            return null;

        _mapper.Map(dto, cart);
        await _carts.SaveChangesAsync();

        var updated = await _carts.GetByIdAsync(id, "User", "CartItems.Product");
        return _mapper.Map<CartDto>(updated!);
    }

    public async Task<CartDto?> PatchAsync(int id, PatchCartDto dto)
    {
        var cart = await _carts.GetByIdAsync(id);
        if (cart == null)
            return null;

        _mapper.Map(dto, cart);
        await _carts.SaveChangesAsync();

        var updated = await _carts.GetByIdAsync(id, "User", "CartItems.Product");
        return _mapper.Map<CartDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cart = await _carts.GetByIdAsync(id);
        if (cart == null)
            return false;

        _carts.Remove(cart);
        await _carts.SaveChangesAsync();
        return true;
    }
}
