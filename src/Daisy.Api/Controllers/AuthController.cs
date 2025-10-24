// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Daisy.Api.Helpers;

namespace Daisy.Api.Controllers
{
    /// <summary>
    /// Controller for authentication and token generation.
    /// Provides OAuth 2.0 compliant JWT token issuance.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// This is a simplified implementation for demonstration purposes.
        /// In production, implement proper user authentication against a user store.
        /// </summary>
        /// <param name="request">The login request with username and password.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        [HttpPost("token")]
        public IActionResult GetToken([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Token request for user: {Username}", LogSanitizer.Sanitize(request.Username));

            if (ValidateUser(request.Username, request.Password))
            {
                var token = GenerateJwtToken(request.Username);
                return Ok(new
                {
                    access_token = token,
                    token_type = "Bearer",
                    expires_in = 3600
                });
            }

            return Unauthorized(new { error = "invalid_grant", error_description = "Invalid username or password" });
        }

        private bool ValidateUser(string username, string password)
        {
            var validUsername = _configuration["Authentication:DefaultUsername"] ?? "admin";
            var validPassword = _configuration["Authentication:DefaultPassword"] ?? "admin";
            
            return username == validUsername && password == validPassword;
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "DaisyApi",
                audience: _configuration["Jwt:Audience"] ?? "DaisyApiClients",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// Request model for login/token endpoint.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
