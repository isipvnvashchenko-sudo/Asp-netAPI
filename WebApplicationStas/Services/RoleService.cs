using AutoMapper;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IRoleService
{
    Task<PagedResult<RoleDto>> GetPagedAsync(int page, int pageSize);
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(CreateRoleDto dto);
    Task<RoleDto?> UpdateAsync(int id, UpdateRoleDto dto);
    Task<RoleDto?> PatchAsync(int id, PatchRoleDto dto);
    Task<bool> DeleteAsync(int id);
}

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roles;
    private readonly IMapper _mapper;

    public RoleService(IRepository<Role> roles, IMapper mapper)
    {
        _roles = roles;
        _mapper = mapper;
    }

    public async Task<PagedResult<RoleDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _roles.GetPagedAsync(page, pageSize, r => r.Id, "Users");
        var total = await _roles.CountAsync();
        return new PagedResult<RoleDto>(_mapper.Map<List<RoleDto>>(items), total);
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _roles.GetByIdAsync(id, "Users");
        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        var role = _mapper.Map<Role>(dto);
        _roles.Add(role);
        await _roles.SaveChangesAsync();
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> UpdateAsync(int id, UpdateRoleDto dto)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return null;

        _mapper.Map(dto, role);
        await _roles.SaveChangesAsync();
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> PatchAsync(int id, PatchRoleDto dto)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return null;

        _mapper.Map(dto, role);
        await _roles.SaveChangesAsync();
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return false;

        _roles.Remove(role);
        await _roles.SaveChangesAsync();
        return true;
    }
}
