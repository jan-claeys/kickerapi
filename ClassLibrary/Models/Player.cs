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
        public int DeffendRating { get; private set; } = 1500;

        [ExcludeFromCodeCoverage]
        //ef
        private Player()
        {

        }

        public Player(string name)
        {
            this.UserName = name;
        }

        public void SetAttackRating(int attackRating)
        {
            this.AttackRating = attackRating;
            SetRating();
        }

        public void SetDeffendRating(int deffendRating)
        {
            this.DeffendRating = deffendRating;
            SetRating();
        }

        private void SetRating()
        {
            this.Rating = (this.AttackRating + this.DeffendRating) / 2;
        }
    }
}
