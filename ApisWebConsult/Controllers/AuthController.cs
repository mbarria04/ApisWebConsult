
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration config, ILogger<AuthController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpPost("token")]
    [AllowAnonymous] 
    public IActionResult GenerarToken([FromHeader(Name = "X-API-KEY")] string apiKey)
    {
        var expected = _config["InternalClient:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(expected) || apiKey != expected)
        {
            _logger.LogWarning("Token rechazado. X-API-KEY recibida: '{apiKey}' | esperada: '{expected}'",
                               apiKey ?? "<null>", expected ?? "<null>");
            return Unauthorized("Cliente no autorizado");
        }

        var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("client", "APP_WEB_7105"),
            new Claim("scope", "internal_api"),
            new Claim(ClaimTypes.Role, "Internal"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var minutes = Convert.ToDouble(_config["Jwt:ExpireMinutes"] ?? "10");

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = jwt });
    }
}
