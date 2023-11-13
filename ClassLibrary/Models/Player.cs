using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary.Models
{
    public class Player : IdentityUser
    {
        const int START_RATING = 1500;
        public int Rating { get; private set; }
        public int AttackRating { get; private set; } = 1500;
        public int DefendRating { get; private set; } = 1500;

        [ExcludeFromCodeCoverage]
        //ef
        private Player()
        {

        }

        public Player(string name)
        {
            UserName = name;

            AttackRating = START_RATING;
            DefendRating = START_RATING;

            SetRating();
        }


        public void SetAttackRating(int rating)
        {
            AttackRating = rating;
            SetRating();
        }

        //add ratingChange to current attackRating
        //returns ratingChange
        public int UpdateAttackRating(double actualOutcome, double expectedOutcome, bool? isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            AttackRating += ratingChange;

            SetRating();

            return ratingChange;
        }

        public void SetDefendRating(int rating)
        {
            DefendRating = rating;
            SetRating();
        }

        //add ratingChange to current defendRating
        //returns ratingChange
        public int UpdateDefendRating(double actualOutcome, double expectedOutcome, bool? isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            DefendRating += ratingChange;

            SetRating();

            return ratingChange;
        }

        //calculate ratingChange based on actual and expected outcome and if player won or lost
        //if draw isWin is null and ratingChange is 0
        //returns ratingchange
        public static int CalcualteRatingChange(double actualOutcome, double expectedOutcome, bool? isWin)
        {
            const int k = 16;
            return isWin == null ? 0 : (int)Math.Ceiling(k * (actualOutcome - expectedOutcome) + (isWin.Value ? k : -k));
        }

        private void SetRating()
        {
            Rating = (AttackRating + DefendRating) / 2;
        }

        public override bool Equals(object? obj)
        {
            return obj is Player player &&
                   Id == player.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
