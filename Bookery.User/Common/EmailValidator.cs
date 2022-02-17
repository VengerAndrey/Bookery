﻿using System.Net.Mail;

namespace Bookery.User.Common;

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