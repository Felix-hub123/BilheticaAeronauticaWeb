using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<List<User>> GetUsersByRoleAsync(string roleName);

        Task<User> GetUserAsync(ClaimsPrincipal user);

        Task<IdentityResult> AddUserAsync(User User, string password);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<User> GetUserByIdAsync(string userId);
        Task LogoutAsync();
        Task<IdentityResult> UpdateUserAsync(User user);
        string GetUserId(ClaimsPrincipal user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);


        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    }
}

