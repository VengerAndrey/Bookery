using Bookery.Node.Common.DTOs.Input;

namespace Bookery.Node.Common.Client;

public interface INodeApiClient
{
    Task<HttpResponseMessage> SignUp(UserSignUpDto userSignUpDto);
}