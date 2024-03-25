using Bookery.Authentication.Common.DTOs.Input;

namespace Bookery.Authentication.Common.Client;

public interface IAuthenticationApiClient
{
    Task<HttpResponseMessage> SignUp(UserSignUpDto userSignUpDto);
}