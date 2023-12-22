using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Geralt;
using Server.Models.Database;
using Server.Utils.Interfaces;

namespace Server.Utils;

// ReSharper disable once InconsistentNaming
public class JWT : IJwt
{ 
        private byte[] Token { get; set; }
        private string Issuer { get; set; }
        private string Audience { get; set; }
        
        private static readonly Encodings.Base64Variant Variant = Encodings.Base64Variant.OriginalNoPadding;
        
        public JWT(string token, string issuer, string audience)
        {
            Token = Encoding.UTF8.GetBytes(token);
            Issuer = issuer;
            Audience = audience;
        }
        
        public static RefreshToken GenerateRefreshToken(string userId)
        {
            Span<byte> buffer = stackalloc byte[24];
            SecureRandom.Fill(buffer);
            var refreshToken = new RefreshToken
            {
                Token = Encodings.ToBase64(buffer, Variant),
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            return refreshToken;
        }
        
        public string CreateToken(string userId)
        {
            var claims = new []
            {
                new Claim(ClaimTypes.PrimarySid, userId),
                new Claim(ClaimTypes.Role, "Base")
            };

            var securityKey = new SymmetricSecurityKey(Token);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        public ClaimsPrincipal? ExpiredTokenInfo(string token)
        {
            var validation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Token)
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }
}