namespace OmnesoftChallenge.DAL.Entities;

public class SystemUser
{
    public SystemUser()
    {
        RefreshToken = new HashSet<RefreshToken>();
    }

    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Active { get; set; } = true;

    public virtual ICollection<RefreshToken> RefreshToken { get; set; }
}

public enum UserRoles
{
    Admin,
    Guest
}
