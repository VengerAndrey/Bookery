namespace Bookery.User.Services.Hash;

public interface IHasher
{
    string Hash(string input);
}