using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary.Models
{
    public class Team
    {
        [ExcludeFromCodeCoverage]
        [Key]
        public int Id { get;  private set; }
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

    }
}
