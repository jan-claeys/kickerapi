using kickerapi.Dtos.Responses.Player;

namespace kickerapi.Dtos.Responses.Match
{
    public class MatchDto
    {
        public int Id { get; set; }
        public TeamDto? PlayerTeam { get; set; }
        public TeamDto? OpponentTeam { get; set; }
        public DateTime Date { get; set; }
        public bool IsCalculatedInRating { get; set; }
        public Position PlayerPosition {  get; set; }

        public class TeamDto
        {
            public int Id { get; set; }
            public PlayerDtoSmall? Attacker { get; set; }
            public PlayerDtoSmall? Defender { get; set; }
            public int AttackerRatingChange { get; set; }
            public int DefenderRatingChange { get; set; }
            public int Score { get; set; }
            public bool IsConfirmed { get; set; }
        }
    }
}
