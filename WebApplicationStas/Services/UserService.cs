using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApplicationStas.DTOs;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize);
    Task<UserDto?> GetByIdAsync(int id);
    Task<User?> GetEntityAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
    Task<UserDto?> PatchAsync(int id, PatchUserDto dto, bool allowRoleChange);
    Task<bool> DeleteAsync(int id);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _users;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> users, IPasswordHasher<User> passwordHasher, IMapper mapper)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize)
    {
        (page, pageSize) = Paging.Normalize(page, pageSize);
        var items = await _users.GetPagedAsync(page, pageSize, u => u.Id, "Role");
        var total = await _users.CountAsync();
        return new PagedResult<UserDto>(_mapper.Map<List<UserDto>>(items), total);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _users.GetByIdAsync(id, "Role");
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public Task<User?> GetEntityAsync(int id) => _users.GetByIdAsync(id);

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        _users.Add(user);
        await _users.SaveChangesAsync();

        var created = await _users.GetByIdAsync(user.Id, "Role");
        return _mapper.Map<UserDto>(created!);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return null;

        _mapper.Map(dto, user);
        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _users.SaveChangesAsync();

        var updated = await _users.GetByIdAsync(id, "Role");
        return _mapper.Map<UserDto>(updated!);
    }

    public async Task<UserDto?> PatchAsync(int id, PatchUserDto dto, bool allowRoleChange)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return null;

        if (!allowRoleChange)
            dto.RoleId = null;

        _mapper.Map(dto, user);
        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _users.SaveChangesAsync();

        var updated = await _users.GetByIdAsync(id, "Role");
        return _mapper.Map<UserDto>(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return false;

        _users.Remove(user);
        await _users.SaveChangesAsync();
        return true;
    }
}
