using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> AddUserAsync(User User, string password);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
    }
}
