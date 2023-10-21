using kickerapi.Dtos.Responses.Player;

namespace kickerapi.Dtos.Responses.Match
{
    public class MatchDto
    {
        public int Id { get; set; }
        public TeamDto? Team1 { get; set; }
        public TeamDto? Team2 { get; set; }
        public DateTime Date { get; set; }

        public class TeamDto
        {
            public int Id { get; set; }
            public PlayerDto? Player1 { get; set; }
            public PlayerDto? Player2 { get; set; }
            public int Score { get; set; }
        }
    }
}
