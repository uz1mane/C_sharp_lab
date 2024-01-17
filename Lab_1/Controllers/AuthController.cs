#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Lab_1.Models;
using Lab_1.Models.DTO;
using Lab_1.Services;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Lab_1.Controllers
{       
    [ApiController]
    public class AuthController : Controller
    {
        private IUserService _userService;
        private IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // POST

        // login
        [HttpPost("/login"), ActionName("Login to existing account")]
        [SwaggerOperation(
            Tags = new[] { "Auth" },
            Summary = "/api/login",
            Description = "Login to existing account. Available only for unauthorized users."            
        )]
        public async Task<IActionResult> LoginUser([FromBody] AuthDTO model)
        {           
            if (!ModelState.IsValid)
            {
                return StatusCode(400, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}");
            }
            try
            {
                var token = await _authService.GenerateToken(model);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(403, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }
        }


        // logout
        [HttpPost("/logout"), ActionName("Logout from your account")]
        [SwaggerOperation(
            Tags = new[] { "Auth" },
            Summary = "/api/logout",
            Description = "Logout from your account. Available only for authorized user"
        )]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            //JwtConfigurations.RuinAllTokens();
            try
            {                
                var id = await _authService.UnsetUserToken(HttpContext);
                return Ok($"User with id {id} has been logged out.");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }
        }


        // register
        [HttpPost("/register"), ActionName("Register new account")]
        [SwaggerOperation(
            Tags = new[] { "Auth" },
            Summary = "/api/register",
            Description = "Register new account. Available only for unauthorized users."
        )]
        public async Task<IActionResult> Register(RegistrationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Model is incorrect");
            }             
            try
            {
                await _userService.CreateUser(model);
                var token = await _authService.GenerateToken(new AuthDTO { Username = model.Username, Password = model.Password });
                return Ok(token);
            }
            catch (Exception ex)
            {              
                return StatusCode(400, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }           
        }
        

    }
}
