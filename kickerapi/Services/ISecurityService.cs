using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace kickerapi.Services
{
    public interface ISecurityService
    {
        Task<Player> FindByNameAsync(string name);
        Task<bool> CheckPasswordAsync(Player player, string password);
        Task<IdentityResult> CreateAsync(Player player, string password);
        JwtSecurityToken GenerateJwtToken(Player player);
        Task<Player> GetUserAsync(ClaimsPrincipal user);
    }
}
