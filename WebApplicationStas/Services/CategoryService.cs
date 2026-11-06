using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface ICategoryService
{
    Task<PagedResult<CategoryDto>> GetPagedAsync(int page, int pageSize);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<CategoryDto?> PatchAsync(int id, PatchCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categories;
    private readonly IMapper _mapper;

    public CategoryService(IRepository<Category> categories, IMapper mapper)
    {
        _categories = categories;
        _mapper = mapper;
    }

    public async Task<PagedResult<CategoryDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _categories.GetPagedAsync(page, pageSize, c => c.Id, "Products");
        var total = await _categories.CountAsync();
        return new PagedResult<CategoryDto>(_mapper.Map<List<CategoryDto>>(items), total);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categories.GetByIdAsync(id, "Products");
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        _categories.Add(category);
        await _categories.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category == null)
            return null;

        _mapper.Map(dto, category);
        await _categories.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> PatchAsync(int id, PatchCategoryDto dto)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category == null)
            return null;

        _mapper.Map(dto, category);
        await _categories.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category == null)
            return false;

        _categories.Remove(category);
        await _categories.SaveChangesAsync();
        return true;
    }
}
