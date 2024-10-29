﻿using AppCupones.Models;
using Elfie.Serialization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppCupones.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(UsuarioModel usuario)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:key"]));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, usuario.ID_Usuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email)
                ]),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials =signingCredentials
            };

            JsonWebTokenHandler jsonWebTokenHandler = new JsonWebTokenHandler();

            return jsonWebTokenHandler.CreateToken(securityTokenDescriptor);

        }
    }
}
