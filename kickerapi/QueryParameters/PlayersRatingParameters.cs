using kickerapi.Dtos;

namespace kickerapi.QueryParameters
{
    public class PlayersRatingParameters: PagingParameters
    {
        public Position? OrderBy { get; set; }
    }
}
