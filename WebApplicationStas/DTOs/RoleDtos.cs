namespace WebApplicationStas.DTOs;

public class RoleShortDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<UserShortDto> Users { get; set; } = new();
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
}

public class PatchRoleDto
{
    public string? Name { get; set; }
}
