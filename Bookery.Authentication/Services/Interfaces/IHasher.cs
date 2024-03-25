namespace Bookery.Authentication.Services.Interfaces;

public interface IHasher
{
    string Hash(string input);
}