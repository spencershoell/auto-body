namespace Shoell.Shared.Exceptions
{
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException() : base() { }
        public UnauthenticatedException(string message) : base(message) { }
        public UnauthenticatedException(string message, Exception inner) : base(message, inner) { }
    }
}
