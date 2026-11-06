using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetPagedAsync(int page, int pageSize);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdateAsync(int id, UpdateOrderDto dto);
    Task<OrderDto?> PatchAsync(int id, PatchOrderDto dto);
    Task<bool> DeleteAsync(int id);
}

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orders;
    private readonly IMapper _mapper;

    public OrderService(IRepository<Order> orders, IMapper mapper)
    {
        _orders = orders;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _orders.GetPagedAsync(page, pageSize, o => o.Id, "User", "OrderItems.Product");
        var total = await _orders.CountAsync();
        return new PagedResult<OrderDto>(_mapper.Map<List<OrderDto>>(items), total);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id, "User", "OrderItems.Product");
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        _orders.Add(order);
        await _orders.SaveChangesAsync();

        var created = await _orders.GetByIdAsync(order.Id, "User", "OrderItems.Product");
        return _mapper.Map<OrderDto>(created!);
    }

    public async Task<OrderDto?> UpdateAsync(int id, UpdateOrderDto dto)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null)
            return null;

        _mapper.Map(dto, order);
        await _orders.SaveChangesAsync();

        var updated = await _orders.GetByIdAsync(id, "User", "OrderItems.Product");
        return _mapper.Map<OrderDto>(updated!);
    }

    public async Task<OrderDto?> PatchAsync(int id, PatchOrderDto dto)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null)
            return null;

        _mapper.Map(dto, order);
        await _orders.SaveChangesAsync();

        var updated = await _orders.GetByIdAsync(id, "User", "OrderItems.Product");
        return _mapper.Map<OrderDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null)
            return false;

        _orders.Remove(order);
        await _orders.SaveChangesAsync();
        return true;
    }
}
