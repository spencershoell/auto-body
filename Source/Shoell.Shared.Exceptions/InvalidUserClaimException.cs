namespace Shoell.Shared.Exceptions
{
    public class InvalidUserClaimException : Exception
    {
        public InvalidUserClaimException() : base() { }
        public InvalidUserClaimException(string message) : base(message) { }
        public InvalidUserClaimException(string message, Exception inner) : base(message, inner) { }
    }
}
