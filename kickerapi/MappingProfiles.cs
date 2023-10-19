﻿using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Player;

namespace kickerapi
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateProjection<Player, PlayerDto>();
        }
    }
}