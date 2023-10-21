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

        public void SetAttackRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            this.AttackRating = CalcualteRating(actualOutcome, expectedOutcome, isWin);
            SetRating();
        }

        public void SetDefendRating(int rating)
        {
            this.DefendRating = rating;
            SetRating();
        }

        public void SetDefendRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            this.DefendRating = CalcualteRating(actualOutcome, expectedOutcome, isWin);
            SetRating();
        }

        private int CalcualteRating(double actualOutcome, double expectedOutcome, bool isWin)
        {
            const int k = 15;
            return (int)Math.Ceiling(this.Rating + k * (actualOutcome - expectedOutcome) + (isWin ? k : -k));
        }

        private void SetRating()
        {
            this.Rating = (this.AttackRating + this.DefendRating) / 2;
        }
    }
}
