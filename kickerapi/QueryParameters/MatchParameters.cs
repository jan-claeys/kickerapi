using kickerapi.Dtos;

namespace kickerapi.QueryParameters
{
    public class MatchParameters : PagingParameters
    {
        public bool IsConfirmed { get; set; } = true;
        public Position? PlayerPosition { get; set; }
    }
}
