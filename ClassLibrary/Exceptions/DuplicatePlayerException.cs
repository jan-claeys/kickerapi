using System.Runtime.Serialization;

namespace ClassLibrary.Exceptions
{
    [Serializable]
    public class DuplicatePlayerException : Exception
    {
        public DuplicatePlayerException() : base("Players must be unique in match")
        {
        }
    }
}