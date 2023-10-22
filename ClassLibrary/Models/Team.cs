using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary.Models
{
    public class Team
    {
        [ExcludeFromCodeCoverage]
        [Key]
        public int Id { get; private set; }
        public Player Attacker { get; private set; }
        public Player Defender { get; private set; }
        public int AttackerRatingChange { get; set; }
        public int DefenderRatingChange { get; set; }
        public int Score { get; set; }

        [ExcludeFromCodeCoverage]
        //ef
        private Team()
        {

        }

        public Team(Player attacker, Player defender, int score)
        {
            this.Attacker = attacker;
            this.Defender = defender;
            this.Score = score;
        }

        public void SetRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            this.AttackerRatingChange = Attacker.UpdateAttackRating(actualOutcome, expectedOutcome, isWin);
            this.DefenderRatingChange = Defender.UpdateDefendRating(actualOutcome, expectedOutcome, isWin);
        }

        public int Rating()
        {
            return (Attacker.AttackRating + Defender.DefendRating) / 2;
        }
    }
}
