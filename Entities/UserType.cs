namespace SimpleBook.Entities;

public enum RoleType
{
    Admin,
    User
}

public class UserRole
{
    public string Id { get; init; }
    public RoleType Role { get; init; }
}