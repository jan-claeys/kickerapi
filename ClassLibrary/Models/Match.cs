using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Match
    {
        [Key]
        public int Id { get; private set; }
        public Team Team1 { get; private set; }
        public Team Team2 { get; private set; }

        //ef
        private Match()
        {

        }

        public Match(Team team1, Team team2)
        {
            this.Team1 = team1;
            this.Team2 = team2;

            this.Team1.Color = Color.Black;
            this.Team2.Color = Color.Green;
        }
    }
}
