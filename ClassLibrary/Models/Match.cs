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
        public bool IsCalculatedInRating { get; private set; } = false;

        [ExcludeFromCodeCoverage]
        //ef
        private Match()
        {

        }

        public Match(Team team1, Team team2)
        {
            Team1 = team1;
            Team2 = team2;

            ArePlayersUnique();

            Date = DateTime.Now;
        }

        //check if players are unique in match otherwise throw exception
        private void ArePlayersUnique()
        {
            var players = GetPlayers();
            var arePlayersUnique = players.Select(x => x.Id).Distinct().Count() == players.Count();

            if (!arePlayersUnique)
            {
                throw new Exception("Players must be unique in match");
            }
        }

        //Update ratings of players in teams if both teams are confirmed
        //returs if rating was updated
        public bool UpdateRatings()
        {
            if (IsCalculatedInRating)
            {
                return false;
            }

            if (!Team1.IsConfirmed || !Team2.IsConfirmed)
            {
                return false;
            }

            var acutualOutcomeTeam1 = CalculateActualOutcome(Team1.Score, Team2.Score);
            var acutualOutcomeTeam2 = CalculateActualOutcome(Team2.Score, Team1.Score);

            var expectedOutcomeTeam1 = CalculateExpectedOutcome(Team1.Rating(), Team2.Rating());
            var expectedOutcomeTeam2 = CalculateExpectedOutcome(Team2.Rating(), Team1.Rating());

            Team1.UpdateRatings(acutualOutcomeTeam1, expectedOutcomeTeam1, Team1.Score == Team2.Score ? null : Team1.Score > Team2.Score);
            Team2.UpdateRatings(acutualOutcomeTeam2, expectedOutcomeTeam2, Team1.Score == Team2.Score ? null :Team2.Score > Team1.Score);

            IsCalculatedInRating = true;

            return true;
        }

        //Calculate actual outcome of the match based on scores of teams
        private static double CalculateActualOutcome(int score1, int score2)
        {
            return score1 + score2 != 0 ? (double)score1 / (double)(score1 + score2) : 0.5;
        }

        //Calculate expected outcome of the match based on ratings of teams
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
