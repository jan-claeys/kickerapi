using AutoMapper;
using ClassLibrary.Models;
using kickerapi.Dtos;
using kickerapi.Dtos.Responses.Match;
using kickerapi.Dtos.Responses.Player;
using Match = ClassLibrary.Models.Match;

namespace kickerapi
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Player, PlayerDto>().ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));
            CreateMap<Player, PlayerDtoSmall>().ForMember(d => d.Name, o => o.MapFrom(s => s.UserName));

            CreateMap<Team, MatchDto.TeamDto>();
            
            CreateMap<Match, MatchDto>().ConvertUsing(new MatchTypeConverter());
        }
    }

    public class MatchTypeConverter : ITypeConverter<Match, MatchDto>
    {
        public MatchDto Convert(Match source, MatchDto destination, ResolutionContext context)
        {
            var playerTeam = source.Team1.Attacker == context.Items["currentPlayer"] || source.Team1.Defender == context.Items["currentPlayer"] ? source.Team1 : source.Team2;
            var opponentTeam = source.Team1.Attacker == context.Items["currentPlayer"] || source.Team1.Defender == context.Items["currentPlayer"] ? source.Team2 : source.Team1;

            var matchDto = new MatchDto
            {
                Id = source.Id,
                Date = source.Date,
                IsCalculatedInRating = source.IsCalculatedInRating,
                PlayerTeam = context.Mapper.Map<MatchDto.TeamDto>(playerTeam),
                OpponentTeam = context.Mapper.Map<MatchDto.TeamDto>(opponentTeam),
                PlayerPosition = playerTeam.Attacker == context.Items["currentPlayer"] ? Position.Attacker : Position.Defender,
            };

            return matchDto;
        }
    }
}
