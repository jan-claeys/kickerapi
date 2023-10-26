using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Requests.Match
{
    public class CreateMatchDto
    {
        [Required]
        public CreateTeamDto Team1 { get; set; }
        [Required]
        public CreateTeamDto Team2 { get; set; }

        public class CreateTeamDto
        {
            [Required]
            public int Score { get; set; }
            [Required]
            public string? AttackerId { get; set; }
            [Required]
            public string? DefenderId { get; set; }
        }
    }
}
