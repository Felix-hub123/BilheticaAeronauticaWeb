using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> AddUserAsync(User User, string password);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<User> GetUserByIdAsync(string userId);
        Task LogoutAsync();
        Task<IdentityResult> UpdateUserAsync(User user);
        string GetUserId(ClaimsPrincipal user);


    }
}
