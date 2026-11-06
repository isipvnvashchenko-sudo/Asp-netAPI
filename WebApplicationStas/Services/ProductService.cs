using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<ProductDto?> PatchAsync(int id, PatchProductDto dto);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly IRepository<Product> _products;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> products, IMapper mapper)
    {
        _products = products;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _products.GetPagedAsync(page, pageSize, p => p.Id, "Category");
        var total = await _products.CountAsync();
        return new PagedResult<ProductDto>(_mapper.Map<List<ProductDto>>(items), total);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _products.GetByIdAsync(id, "Category");
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        _products.Add(product);
        await _products.SaveChangesAsync();

        var created = await _products.GetByIdAsync(product.Id, "Category");
        return _mapper.Map<ProductDto>(created!);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
            return null;

        _mapper.Map(dto, product);
        await _products.SaveChangesAsync();

        var updated = await _products.GetByIdAsync(id, "Category");
        return _mapper.Map<ProductDto>(updated!);
    }

    public async Task<ProductDto?> PatchAsync(int id, PatchProductDto dto)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
            return null;

        _mapper.Map(dto, product);
        await _products.SaveChangesAsync();

        var updated = await _products.GetByIdAsync(id, "Category");
        return _mapper.Map<ProductDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
            return false;

        _products.Remove(product);
        await _products.SaveChangesAsync();
        return true;
    }
}
