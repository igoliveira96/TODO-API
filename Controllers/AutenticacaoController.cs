using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using TODO_API.Models;
using TODO_API.Service;
using System.Threading.Tasks;

namespace TODO_API.Controllers
{
    [Route("api/[controller]")]
    public class AutenticacaoController : Controller
    {
        IUsuarioService _service;
        
        public AutenticacaoController(IUsuarioService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<object> TokenAcesso(
            [FromBody]Usuario usuario,
            [FromServices]SigningConfigurations signingConfigurations)
        {
            bool credenciaisValidas = false;
            if (usuario != null 
                && !String.IsNullOrWhiteSpace(usuario.Id)
                && !String.IsNullOrWhiteSpace(usuario.Token))
            {
                var usuarioBase = await _service.FindUserAsync(usuario.Id, usuario.Token);
                credenciaisValidas = usuarioBase != null ? true : false;
            }
            
            if (credenciaisValidas)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.Id, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Id)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(120);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = "http://localhost:5000",
                    Audience = "http://localhost:5000",
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

                return new
                {
                    authenticated = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Falha ao autenticar"
                };
            }
        }
    }
}