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
using Lab_1.Services;
using Microsoft.AspNetCore.Authorization;
using Lab_1.Models.DTO;

namespace Lab_1.Controllers
{   
    [ApiController]
    [Route("users")]

    public class UsersController : Controller
    {        
        private readonly Context _context;
        private IUserService _userService;
        private IRolesService _rolesService;

        public UsersController(Context context, IUserService userService, IRolesService rolesService)
        {
            _context = context;
            _userService = userService;
            _rolesService = rolesService;
        }

        // GET
        
        [HttpGet, ActionName("List of all users (admin only)")]            
        [SwaggerOperation(
            Tags = new[] { "User Data" },
            Summary = "/api/users",
            Description = "List of all users in system. Available only for admin"
        )]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsersInfo(string token)
        {
            var list = await _userService.GetAllUsersList();
            return Ok(list);                                   
        }
        
        [HttpGet("{userId}"), ActionName("Get detailed data for specified user")]
        [SwaggerOperation(
            Tags = new[] { "User Data" },
            Summary = "/api/users/{userId}",
            Description = "Get detailed data for specified user. Available only for admin or if (requestUserId == userId)"
        )]        
        public async Task<IActionResult> GetUserInfo(int userId)
        {        
            if (_userService.CheckIfUserChecksHimself(userId, HttpContext) || _userService.CheckIfUserIsAdmin(HttpContext))
            {
                try
                {
                    var userInfo = await _userService.GetUserInfoByUserId(userId);
                    return Ok(userInfo);
                }
                catch (Exception ex)
                {
                    return StatusCode(404, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
                }
            }
            else
            {
                return StatusCode(403, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}");
            }
        }

        // PATCH

        [HttpPatch("{userId}"), ActionName("Edit data for concrete user")]
        [SwaggerOperation(
            Tags = new[] { "User Data" },
            Summary = "/api/users/{userId}",
            Description = "Edit data for concrete user (except edit role)"
        )]
        [Authorize]
        public async Task<IActionResult> EditUserData(EditUserDataDTO editData ,int userId)
        {
            try
            {
                var userInfo = await _userService.EditUserData(editData, userId);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }            
        }

        // DELETE

        [HttpDelete("{userId}"), ActionName("Delete concrete user")]
        [SwaggerOperation(
            Tags = new[] { "User Data" },
            Summary = "/api/users/{userId}",
            Description = "Delete concrete user (except edit role). Available only for admin"
        )]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> DeleteUser(int userId)
        {            
            try
            {
                await _userService.DeleteUser(userId);
                return Ok($"User with id {userId} was successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }

        }

        // POST

        [HttpPost("{userId}/role"), ActionName("Edit role for specified user")]
        [SwaggerOperation(
            Tags = new[] { "User Data" },
            Summary = "/api/users/{userId}/role",
            Description = "Edit role for specified user. Available only for admin"
        )]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUserRole(EditRoleDTO role, int userId)
        {
            try
            {
                var newRole = await _rolesService.EditUserRole(role, userId);
                return Ok($"Success. User with id {userId} is now {newRole}");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"Something went wrong in method {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name}. {ex}");
            }
        }        

    }
}
