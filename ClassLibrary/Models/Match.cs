using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary.Models
{
    public class Match
    {
        [ExcludeFromCodeCoverage]
        [Key]
        public int Id { get; private set; }
        public Team Team1 { get; private set; }
        public Team Team2 { get; private set; }
        public DateTime Date { get; private set; }

        [ExcludeFromCodeCoverage]
        //ef
        private Match()
        {

        }

        public Match(Team team1, Team team2)
        {
            this.Team1 = team1;
            this.Team2 = team2;

            ArePlayersUnique();

            this.Date = DateTime.Now;
        }

        private void ArePlayersUnique()
        {
            var players = GetPlayers();
            var arePlayersUnique = players.Select(x => x.Id).Distinct().Count() == players.Count();

            if(!arePlayersUnique)
            {
                throw new Exception("Players must be unique in match");
            }
        }

        public IEnumerable<Player> GetPlayers()
        {
            return new List<Player> { Team1.Attacker, Team1.Deffender, Team2.Attacker, Team2.Deffender };
        }
    }
}
