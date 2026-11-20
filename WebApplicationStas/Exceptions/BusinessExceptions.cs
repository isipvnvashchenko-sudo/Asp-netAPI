namespace WebApplicationStas.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

public class AccountDisabledException : Exception
{
    public AccountDisabledException(string message) : base(message)
    {
    }
}
