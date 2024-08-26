public interface IAuthenticationService
{
    bool Authenticate(string token);
}

public class AuthenticationService : IAuthenticationService
{
    public bool Authenticate(string token)
    {
        // Implement your authentication logic here
        // For now, return true to allow all requests
        return true;
    }
}
