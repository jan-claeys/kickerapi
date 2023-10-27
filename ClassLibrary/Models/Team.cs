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
        public int? AttackerRatingChange { get; private set; }
        public int? DefenderRatingChange { get; private set; }
        public int Score { get; set; }
        public bool IsConfirmed { get; private set; } = false;

        [ExcludeFromCodeCoverage]
        //ef
        private Team()
        {

        }

        public Team(Player attacker, Player defender, int score)
        {
            Attacker = attacker;
            Defender = defender;
            Score = score;
        }

        //update ratings of players in team and set ratingChange
        public void UpdateRatings(double actualOutcome, double expectedOutcome, bool isWin)
        {
            AttackerRatingChange = Attacker.UpdateAttackRating(actualOutcome, expectedOutcome, isWin);
            DefenderRatingChange = Defender.UpdateDefendRating(actualOutcome, expectedOutcome, isWin);
        }

        public int Rating()
        {
            return (Attacker.AttackRating + Defender.DefendRating) / 2;
        }

        public void Confirm()
        {
            IsConfirmed = true;
        }
    }
}
