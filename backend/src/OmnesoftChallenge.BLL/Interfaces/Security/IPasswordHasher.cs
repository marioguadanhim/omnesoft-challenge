namespace OmnesoftChallenge.BLL.Interfaces.Security;

public interface IPasswordHasher
{
    string HashPasswordWithKey(string password);
}
