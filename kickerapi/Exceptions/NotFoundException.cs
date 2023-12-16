using System.Runtime.Serialization;

namespace kickerapi.Services
{
    [Serializable]
    internal class NotFoundException : Exception
    {
        public NotFoundException(string? message) : base(message)
        {
        }
    }
}