namespace kickerapi.QueryParameters
{
    public class MatchParameters : PagingParameters
    {
        public bool IsConfirmed { get; set; } = true;
    }
}
