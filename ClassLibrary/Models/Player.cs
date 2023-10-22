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
            this.UserName = name;
        }


        public void SetAttackRating(int rating)
        {
            this.AttackRating = rating;
            SetRating();
        }

        //returns gained rating
        public int UpdateAttackRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            this.AttackRating +=  ratingChange;

            SetRating();

            return ratingChange;
        }

        public void SetDefendRating(int rating)
        {
            this.DefendRating = rating;
            SetRating();
        }

        //returns lost rating
        public int UpdateDefendRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            var ratingChange = CalcualteRatingChange(actualOutcome, expectedOutcome, isWin);
            this.DefendRating += ratingChange;
            
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
            this.Rating = (this.AttackRating + this.DefendRating) / 2;
        }
    }
}
