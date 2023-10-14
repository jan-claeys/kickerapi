﻿using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Player
    {
        [Key]
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int AttackRating { get; set; }
        public int DeffendRating { get; set; }

        //ef
        private Player()
        {

        }

        public Player(string name)
        {
            this.Name = name;
        }

        public int Rating()
        {
            return (this.AttackRating + this.DeffendRating) / 2;
        }
    }
}