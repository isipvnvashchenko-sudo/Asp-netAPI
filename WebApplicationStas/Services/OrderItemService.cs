using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IOrderItemService
{
    Task<PagedResult<OrderItemDto>> GetPagedAsync(int page, int pageSize);
    Task<OrderItemDto?> GetByIdAsync(int id);
    Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto);
    Task<OrderItemDto?> UpdateAsync(int id, UpdateOrderItemDto dto);
    Task<OrderItemDto?> PatchAsync(int id, PatchOrderItemDto dto);
    Task<bool> DeleteAsync(int id);
}

public class OrderItemService : IOrderItemService
{
    private readonly IRepository<OrderItem> _orderItems;
    private readonly IMapper _mapper;

    public OrderItemService(IRepository<OrderItem> orderItems, IMapper mapper)
    {
        _orderItems = orderItems;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderItemDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _orderItems.GetPagedAsync(page, pageSize, oi => oi.Id, "Product");
        var total = await _orderItems.CountAsync();
        return new PagedResult<OrderItemDto>(_mapper.Map<List<OrderItemDto>>(items), total);
    }

    public async Task<OrderItemDto?> GetByIdAsync(int id)
    {
        var orderItem = await _orderItems.GetByIdAsync(id, "Product");
        return orderItem == null ? null : _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto)
    {
        var orderItem = _mapper.Map<OrderItem>(dto);
        _orderItems.Add(orderItem);
        await _orderItems.SaveChangesAsync();

        var created = await _orderItems.GetByIdAsync(orderItem.Id, "Product");
        return _mapper.Map<OrderItemDto>(created!);
    }

    public async Task<OrderItemDto?> UpdateAsync(int id, UpdateOrderItemDto dto)
    {
        var orderItem = await _orderItems.GetByIdAsync(id);
        if (orderItem == null)
            return null;

        _mapper.Map(dto, orderItem);
        await _orderItems.SaveChangesAsync();

        var updated = await _orderItems.GetByIdAsync(id, "Product");
        return _mapper.Map<OrderItemDto>(updated!);
    }

    public async Task<OrderItemDto?> PatchAsync(int id, PatchOrderItemDto dto)
    {
        var orderItem = await _orderItems.GetByIdAsync(id);
        if (orderItem == null)
            return null;

        _mapper.Map(dto, orderItem);
        await _orderItems.SaveChangesAsync();

        var updated = await _orderItems.GetByIdAsync(id, "Product");
        return _mapper.Map<OrderItemDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderItem = await _orderItems.GetByIdAsync(id);
        if (orderItem == null)
            return false;

        _orderItems.Remove(orderItem);
        await _orderItems.SaveChangesAsync();
        return true;
    }
}
