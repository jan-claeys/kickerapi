using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Team
    {
        [Key]
        public int Id { get;  private set; }
        public Player Attacker { get; private set; }
        public Player Deffender { get; private set; }
        public int Score { get; set; }
        public Color Color { get; internal set; }

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
