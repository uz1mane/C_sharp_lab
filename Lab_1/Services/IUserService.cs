using Lab_1.Models;
using Lab_1.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Services
{
    public interface IUserService
    {
        Task CreateUser(RegistrationDTO model);
        Task<List<User>> GetAllUsersList();
        Task<DetailedUserInfoDTO> GetUserInfoByUserId(int id);
        Task<DetailedUserInfoDTO> EditUserData(EditUserDataDTO userData, int id);
        Task DeleteUser(int id);
        bool CheckIfUserChecksHimself(int userId, HttpContext context);
        bool CheckIfUserIsAdmin(HttpContext context);
    }

    public class UserService : IUserService
    {
        private readonly Context _context;

        public UserService(Context context)
        {
            _context = context;
        }
        public async Task CreateUser(RegistrationDTO model)
        {
            await _context.AddAsync(new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Username = model.Username,
                Password = model.Password,
                Role = "admin",
            });
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsersList()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<DetailedUserInfoDTO> GetUserInfoByUserId(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != default)
            {
                return new DetailedUserInfoDTO
                {                    
                    Username = user.Username,
                    Id = user.Id,
                    Role = user.Role,
                    Name = user.Name,
                    Surname = user.Surname
                };
            }
            else throw new Exception("User with such ID was not found");
        }

        public async Task<DetailedUserInfoDTO> EditUserData(EditUserDataDTO userData, int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != default)
            {
                if (userData.Password != null)
                    user.Password = userData.Password;
                if (userData.Surname != null)
                    user.Surname = userData.Surname;
                if (userData.Name != null)
                    user.Name = userData.Name;
                await _context.SaveChangesAsync();
                return await GetUserInfoByUserId(id);
            }
            else throw new Exception("User with such ID was not found");
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != default)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            } 
            else throw new Exception("User with such ID was not found.");            
        }

        public bool CheckIfUserChecksHimself(int userId, HttpContext context)
        {
            var authenticatedUserId = context.User.Claims.Where(c => c.Type == "id")
                                        .Select(c => c.Value).SingleOrDefault();
            if (authenticatedUserId == userId.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIfUserIsAdmin(HttpContext context)
        {
            var role = context.User.Claims.Where(c => c.Type == "role")
                                .Select(c => c.Value).SingleOrDefault();
            if (role == "admin")
            {
                return true;
            }
            else
            {
                return false;
            }            
        }
    }
}
