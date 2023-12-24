namespace AuthService.DataAccess.Objects;

public class Permission
{
    public Guid Id { get; set; }
    public string PermissionName { get; set; }
    public List<Role> Roles { get; set; }
}