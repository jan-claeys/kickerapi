namespace kickerapi.QueryParameters
{
    public class PlayersParameters: PagingParameters
    {
        public string OrderBy { get; set; } = "Rating";
    }
}
