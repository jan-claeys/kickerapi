using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

            UpdateRating();

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

        private void UpdateRating()
        {
            var acutualOutcomeTeam1 = CalculateActualOutcome(Team1.Score, Team2.Score);
            var acutualOutcomeTeam2 = CalculateActualOutcome(Team2.Score, Team1.Score);

            var expectedOutcomeTeam1 = CalculateExpectedOutcome(Team1.Rating(), Team2.Rating());
            var expectedOutcomeTeam2 = CalculateExpectedOutcome(Team2.Rating(), Team1.Rating());

            Team1.SetRating(acutualOutcomeTeam1, expectedOutcomeTeam1, Team1.Score > Team2.Score);
            Team2.SetRating(acutualOutcomeTeam2, expectedOutcomeTeam2, Team2.Score > Team1.Score);
        }

        private static double CalculateActualOutcome(int score1, int score2)
        {
            return score1 + score2 != 0 ? (double)score1 / (double)(score1 + score2) : 0.5;
        }
        
        private static double CalculateExpectedOutcome(int rating1, int rating2)
        {
            const int c = 400;
            return Math.Pow(10, (double)rating1 / (double)c) / (Math.Pow(10, (double)rating1 / (double)c) + Math.Pow(10, (double)rating2 / (double)c));
        }

        public IEnumerable<Player> GetPlayers()
        {
            return new List<Player> { Team1.Attacker, Team1.Defender, Team2.Attacker, Team2.Defender };
        }
    }
}
