using System.ComponentModel.DataAnnotations;
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

            RecalculateRating();

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

        private void RecalculateRating()
        {
            const int c = 400;

            var acutualOutcomeTeam1 = Team1.Score + Team2.Score != 0 ?Team1.Score / (Team1.Score + Team2.Score) : 0.5;
            var acutualOutcomeTeam2 = Team1.Score + Team2.Score != 0 ?Team2.Score / (Team1.Score + Team2.Score) : 0.5;

            var expectedOutcomeTeam1 = Math.Pow(10, Team1.Rating() / c) / (Math.Pow(10, Team1.Rating() / c) + Math.Pow(10, Team2.Rating() / c));
            var expectedOutcomeTeam2 = Math.Pow(10, Team2.Rating() / c) / (Math.Pow(10, Team1.Rating() / c) + Math.Pow(10, Team2.Rating() / c));

            Team1.RecalculateRating(expectedOutcomeTeam1, acutualOutcomeTeam1);
            Team2.RecalculateRating(expectedOutcomeTeam2, acutualOutcomeTeam2);
        }

        public IEnumerable<Player> GetPlayers()
        {
            return new List<Player> { Team1.Attacker, Team1.Deffender, Team2.Attacker, Team2.Deffender };
        }
    }
}
