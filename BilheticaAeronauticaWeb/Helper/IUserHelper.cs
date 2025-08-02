using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Interface que define métodos auxiliares para operações relacionadas com utilizadores,
    /// incluindo autenticação, autorização, criação, atualização e gestão de roles e tokens.
    /// </summary>
    public interface IUserHelper
    {
        /// <summary>
        /// Verifica se o email do utilizador foi confirmado.
        /// </summary>
        /// <param name="user">Usuário a verificar.</param>
        /// <returns>True se o email estiver confirmado, caso contrário false.</returns>
        Task<bool> IsEmailConfirmedAsync(User user);


        /// <summary>
        /// Efetua o login do utilizador com os dados fornecidos no ViewModel.
        /// </summary>
        /// <param name="model">Informações para login (email, password, etc.).</param>
        /// <returns>Resultado da tentativa de login.</returns>
        Task<SignInResult> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Obtém um utilizador pelo seu email.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <returns>Instância do utilizador ou null se não existir.</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Obtém todos os utilizadores associados a uma role específica.
        /// </summary>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>Lista de utilizadores com a role especificada.</returns>
        Task<List<User>> GetUsersByRoleAsync(string roleName);

        /// <summary>
        /// Obtém as roles associadas a um determinado utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Lista de nomes das roles do utilizador.</returns>
        Task<IList<string>> GetUserRolesAsync(User user);


        /// <summary>
        /// Obtém a entidade utilizador a partir do ClaimsPrincipal.
        /// </summary>
        /// <param name="user">ClaimsPrincipal do utilizador.</param>
        /// <returns>Instância do utilizador correspondente.</returns>
        Task<User> GetUserAsync(ClaimsPrincipal user);


        /// <summary>
        /// Cria um novo utilizador com a password fornecida.
        /// </summary>
        /// <param name="user">Entidade do utilizador a adicionar.</param>
        /// <param name="password">Password do utilizador.</param>
        /// <returns>Resultado da criação.</returns>
        Task<IdentityResult> AddUserAsync(User User, string password);


        /// <summary>
        /// Adiciona um utilizador a uma role específica.
        /// </summary>
        /// <param name="user">Utilizador a adicionar.</param>
        /// <param name="roleName">Nome da role para atribuir.</param>
        Task<IdentityResult> AddUserToRoleAsync(User user, string role);

        /// <summary>
        /// Verifica se o utilizador pertence a uma role específica.
        /// </summary>
        /// <param name="user">Utilizador a verificar.</param>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>True se o utilizador está na role; caso contrário, false.</returns>
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        /// <summary>
        /// Obtém um utilizador pelo seu identificador.
        /// </summary>
        /// <param name="userId">Identificador do utilizador.</param>
        /// <returns>Instância do utilizador ou null se não existir.</returns>
        Task<User> GetUserByIdAsync(string userId);

        /// <summary>
        /// Efetua logout do utilizador atual.
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Atualiza os dados de um utilizador existente.
        /// </summary>
        /// <param name="user">Utilizador com dados atualizados.</param>
        /// <returns>Resultado da operação de atualização.</returns>
        Task<IdentityResult> UpdateUserAsync(User user);

        /// <summary>
        /// Obtém o Id do utilizador a partir do ClaimsPrincipal.
        /// </summary>
        /// <param name="user">ClaimsPrincipal do utilizador.</param>
        /// <returns>String que representa o Id do utilizador.</returns>
        string GetUserId(ClaimsPrincipal user);

      


        /// <summary>
        /// Altera a password do utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="oldPassword">Password atual.</param>
        /// <param name="newPassword">Nova password.</param>
        /// <returns>Resultado da operação de alteração.</returns>
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);


        /// <summary>
        /// Cria a role caso esta não exista no sistema.
        /// </summary>
        /// <param name="roleName">Nome da role a verificar/criar.</param>
        Task CheckRoleAsync(string roleName);

        /// <summary>
        /// Valida a password de um utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="password">Password a validar.</param>
        /// <returns>Resultado da validação.</returns>
        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        /// <summary>
        /// Gera um token para confirmação do email do utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Token gerado para confirmação de email.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        /// <summary>
        /// Confirma o email do utilizador usando o token enviado.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="token">Token para confirmar o email.</param>
        /// <returns>Resultado da operação de confirmação.</returns>
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        /// <summary>
        /// Gera um token para redefinição de password do utilizador.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <returns>Token para reset de password.</returns>
        Task<string> GeneratePasswordResetTokenAsync(User user);

        /// <summary>
        /// Redefine a password do utilizador usando o token fornecido.
        /// </summary>
        /// <param name="user">Utilizador alvo.</param>
        /// <param name="token">Token de redefinição da password.</param>
        /// <param name="password">Nova password.</param>
        /// <returns>Resultado da operação de redefinição.</returns>
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    }
}

