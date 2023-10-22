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
            public PlayerDto? Attacker { get; set; }
            public PlayerDto? Defender { get; set; }
            public int AttackerRatingChange { get; set; }
            public int DefenderRatingChange { get; set; }
            public int Score { get; set; }
        }
    }
}
