using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos.Responses.Match;
using kickerapi.Dtos.Responses.Player;

namespace kickerapi
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateProjection<Player, PlayerDto>().ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
            CreateProjection<Player, PlayerDtoSmall>().ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
            CreateProjection<Match, MatchDto>();
            CreateProjection<Team, MatchDto.TeamDto>();
        }
    }
}
