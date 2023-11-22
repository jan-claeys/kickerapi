﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace kickerapi.Dtos.Requests.Match
{
    public class CreateMatchDto
    {
        [Required]
        public Position PlayerPosition { get; set; }
        [Required]
        public string AllyId { get; set; } = null!;
        [Required]
        public string OpponentAttackerId { get; set; } = null!;
        [Required]
        public string OpponentDefenderId { get; set; } = null!;
        [Required]
        public int PlayerScore { get; set; }
        [Required]
        public int OpponentScore { get; set; }
    }
}
