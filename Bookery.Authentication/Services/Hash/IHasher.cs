namespace Bookery.Authentication.Services.Hash;

public interface IHasher
{
    string Hash(string input);
}