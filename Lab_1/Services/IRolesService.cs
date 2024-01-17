using Lab_1.Models;
using Lab_1.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Services
{
    public interface IRolesService
    {
        Task<string> EditUserRole(EditRoleDTO role, int id);
    }

    public class RolesService : IRolesService
    {
        private readonly Context _context;

        public RolesService(Context context)
        {
            _context = context;
        }

        public async Task<string> EditUserRole(EditRoleDTO role, int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != default)
            {
                user.Role = role.Role;
                await _context.SaveChangesAsync();
                return user.Role;
            }
            else throw new Exception("User with such ID was not found.");
        }
    }
}
