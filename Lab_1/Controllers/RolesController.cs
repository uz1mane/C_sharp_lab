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

namespace Lab_1.Controllers
{
    [ApiController]
    [Route("roles")]
    public class RolesController : Controller
    {
        private readonly Context _context;

        public RolesController(Context context)
        {
            _context = context;
        }

        // GET

        [HttpPost, ActionName("List of all roles in system")]
        [SwaggerOperation(
            Tags = new[] { "Roles" },
            Summary = "/api/roles",
            Description = "List of all roles in system. Available only for authorized users"
        )]
        public string GetAllRoles()
        {
            return "This is /roles";
        }

        [HttpPost("{roleId}"), ActionName("Get selected role")]
        [SwaggerOperation(
            Tags = new[] { "Roles" },
            Summary = "/api/roles/{roleId}",
            Description = "Get selected role. Available only for authorized users"
        )]
        public string GetSelectedRole()
        {
            return "This is /roles";
        }
    }
}
