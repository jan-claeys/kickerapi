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
        public Player Deffender { get; private set; }
        public int Score { get; set; }

        [ExcludeFromCodeCoverage]
        //ef
        private Team()
        {

        }

        public Team(Player attacker, Player deffender, int score)
        {
            this.Attacker = attacker;
            this.Deffender = deffender;
            this.Score = score;
        }

        public void RecalculateRating(double actualOutcome, double expectedOutcome)
        {
            const int k = 32;

            Attacker.SetAttackRating((int)Math.Ceiling(Attacker.AttackRating + k * (actualOutcome - expectedOutcome)));
            Deffender.SetDeffendRating((int)Math.Ceiling(Deffender.DeffendRating + k * (actualOutcome - expectedOutcome)));
        }

        public int Rating()
        {
            return (Attacker.AttackRating + Deffender.DeffendRating) / 2;
        }
    }
}
