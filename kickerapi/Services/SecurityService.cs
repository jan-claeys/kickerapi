using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace kickerapi.Services
{
    public class SecurityService: ISecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Player> _userManager;

        public SecurityService(IConfiguration configuration, UserManager<Player> userManager)
        {
            this._configuration = configuration;
            this._userManager = userManager;
        }

        public async Task<bool> CheckPasswordAsync(Player player, string password)
        {
            return await _userManager.CheckPasswordAsync(player, password);
        }

        public async Task<IdentityResult> CreateAsync(Player player, string password)
        {
            return await _userManager.CreateAsync(player, password);
        }

        public async Task<Player> FindByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }
        
        public async Task<Player> GetUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public JwtSecurityToken GenerateJwtToken(Player player)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, player.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
