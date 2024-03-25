using System.Net.Mail;

namespace Bookery.User.Validators;

public class EmailValidator
{
    public static bool Validate(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}