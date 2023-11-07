﻿using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Requests.Match
{
    public class CreateMatchDto
    {
        [Required]
        public Position PlayerPosition { get; set; }
        [Required]
        public string AllyId { get; set; }
        [Required]
        public int PlayerScore { get; set;}
        [Required]
        public string OpponentAttackerId { get; set; }
        [Required]
        public string OpponentDefenderId { get; set; }
        [Required]
        public int OpponentScore { get; set; }

        public enum Position
        {
            Attacker,
            Defender
        }
    }
}
