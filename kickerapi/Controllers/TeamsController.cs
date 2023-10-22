using kickerapi.Dtos.Responses.Match;
using kickerapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kickerapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly KickerContext _context;
        private readonly ISecurityService _securityService;
        private readonly IMatchService _matchService;

        public TeamsController(KickerContext context, ISecurityService securityService, IMatchService matchService)
        {
            _context = context;
            _securityService = securityService;
            _matchService = matchService;
        }

        [HttpPut("confirm/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IStatusCodeActionResult> Confirm(int id)
        {
            try
            {
                var player = await _securityService.GetUserAsync(User);
                var team = await _context.Teams.FindAsync(id) ?? throw new Exception("Team not found");

                if (team.Attacker.Id != player.Id && team.Defender.Id != player.Id)
                    throw new Exception("You are not allowed to confirm this team");
                
                team.Confirm();

                var matchesToUpdate = await _matchService.GetMatches(player, false).ToListAsync();
                foreach (var match in matchesToUpdate)
                {
                    var isUpdated = match.UpdateRatings();
                    if(!isUpdated)
                        break;
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<TeamsControllerr>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
