namespace kickerapi.Exceptions
{
    [Serializable]
    internal class NotAllowedException : Exception
    {
        public NotAllowedException(string? message) : base(message)
        {
        }
    }
}