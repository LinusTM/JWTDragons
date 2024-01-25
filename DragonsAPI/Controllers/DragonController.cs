using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DragonAPI.Controllers;

[ApiController]
[Route("dragon")]
public class DragonController : ControllerBase
{
    private static readonly IConfiguration JwtConfig = Configuration.GetConfig().GetSection("JWT");
    private static readonly SymmetricSecurityKey JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig["Key"] ?? throw new Exception("Invalid appsettings.json")));
    private static readonly double JwtLifetime = Double.Parse(JwtConfig["Lifetime"] ?? throw new Exception("Invalid appsettings.json"));

    private static readonly string[] DragonTunes = new[]
    {
        "Dragon's Midnight Flight",
        "Whispers of the Drage",
        "Dragonfire Serenade",
        "Drage Dancing under the Stars",
        "The Dragon and the Dreamer",
        "Echoes of a Drage's Roar",
        "Dragon's Lullaby",
        "Sapphire Drage Skies",
        "The Drage's Secret Melody",
        "Realm of the Fire Dragon",
    };

    // TODO: Implememnt JWT
    [Authorize]
    [HttpGet("dragontunes")]
    public string[] GetDragonMusic()
    {
        return DragonTunes;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] DragonLoginModel dragonLoginModel)
    {
        Dragon? dragon = DragonDB.Fetch(dragonLoginModel.Name);

        // Check if dragon exists or password is correct in db
        if (dragon == null || !dragon.VerifyPassword(dragonLoginModel.Password))
        {
            // Return 400
            return BadRequest($"Password or dragon is invalid.");
        }

        // JWT fields
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, dragonLoginModel.Name),
        };

        // Create the token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(JwtLifetime),
            signingCredentials: new SigningCredentials(JwtKey, SecurityAlgorithms.HmacSha256)
        );

        // Serialize token
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Return 200
        return Ok(new { Token = tokenString });
    }

    [HttpPut("register")]
    // I'm reusing the model, this should be seperate in practice
    public IActionResult Register([FromBody] DragonLoginModel dragonLoginModel)
    {
        Dragon? dragon = DragonDB.Fetch(dragonLoginModel.Name);

        // Check if dragon exists in db
        if (dragon != null)
        {
            return BadRequest($"Dragon {dragon.Name} already exists.");
        }

        // Register new dragon
        Dragon newDragon = new Dragon(dragonLoginModel.Name, dragonLoginModel.Password);
        DragonDB.Register(newDragon);

        // Return 204, 201 for a redirect (login page e.g.)
        return NoContent();
    }
}

