using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ClassLibrary.Models
{
    public class Player: IdentityUser
    {
        [Key]
        public int Id { get; set; }
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
        }


        public void SetAttackRating(int rating)
        {
            AttackRating = rating;
            SetRating();
        }

        //returns gained rating
        public int UpdateAttackRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            AttackRating +=  ratingChange;

            SetRating();

            return ratingChange;
        }

        public void SetDefendRating(int rating)
        {
            DefendRating = rating;
            SetRating();
        }

        //returns lost rating
        public int UpdateDefendRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            DefendRating += ratingChange;
            
            SetRating();

            return ratingChange;
        }

        public static int CalcualteRatingChange(double actualOutcome, double expectedOutcome, bool isWin)
        {
            const int k = 16;
            return (int)Math.Ceiling(k * (actualOutcome - expectedOutcome) + (isWin ? k : -k));
        }

        private void SetRating()
        {
            Rating = (AttackRating + DefendRating) / 2;
        }
    }
}
