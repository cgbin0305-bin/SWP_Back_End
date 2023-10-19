namespace Errors;

public class EmailOrPhoneIsTakenException : Exception
{
    public EmailOrPhoneIsTakenException() : base("Email or phone number is taken")
    {
    }
}