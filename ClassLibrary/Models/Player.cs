using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Player
    {
        [Key]
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int AttackRating { get; set; }
        public int DeffendRating { get; set; }
        public string Password { get; private set; }

        //ef
        private Player()
        {

        }

        public Player(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }

        public int Rating()
        {
            return (this.AttackRating + this.DeffendRating) / 2;
        }
    }
}
