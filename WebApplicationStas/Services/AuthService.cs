using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApplicationStas.DTOs;
using WebApplicationStas.Exceptions;
using WebApplicationStas.Models;
using WebApplicationStas.Repositories;

namespace WebApplicationStas.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<Role> _roles;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(
        IRepository<User> users,
        IRepository<Role> roles,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        IMapper mapper)
    {
        _users = users;
        _roles = roles;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _users.GetFirstOrDefaultAsync(u => u.Username == dto.Username);
        if (existing != null)
            throw new BusinessRuleException($"Логин \"{dto.Username}\" уже занят.");

        var role = await _roles.GetFirstOrDefaultAsync(r => r.Name == "User");
        if (role == null)
            throw new NotFoundException("Роль по умолчанию \"User\" не найдена в базе данных.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            IsActive = true,
            RoleId = role.Id
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _users.Add(user);
        await _users.SaveChangesAsync();

        var created = await _users.GetByIdAsync(user.Id, "Role");
        return CreateResponse(created!);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetFirstOrDefaultAsync(u => u.Username == dto.Username, "Role");
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return null;

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verification == PasswordVerificationResult.Failed)
            return null;

        if (!user.IsActive)
            throw new AccountDisabledException("Учётная запись отключена. Обратитесь к администратору.");

        return CreateResponse(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _users.GetByIdAsync(userId, "Role");
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    private AuthResponseDto CreateResponse(User user)
    {
        var expiresMinutes = _configuration.GetValue("Jwt:ExpiresMinutes", 60);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role?.Name ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }
}
