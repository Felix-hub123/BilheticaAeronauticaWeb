using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Data
{
    public class IdentitySeed
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Funcionario", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userHelper = serviceProvider.GetRequiredService<UserHelper>();
            var email = "admin@empresa.com";
            var user = await userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Nome = "Admin",
                    Apelido = "Plataforma",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };
                await userHelper.AddUserAsync(user, "Admin123!");
                await userHelper.AddUserToRoleAsync(user, "Admin");
            }
        }


        public static async Task SeedFuncionarioAsync(IServiceProvider serviceProvider)
        {
            var userHelper = serviceProvider.GetRequiredService<UserHelper>();
            var email = "funcionario@empresa.com";
            var user = await userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Nome = "Funcionario",
                    Apelido = "Exemplo",
                    Email = email,
                    UserName = email,
                    EmailConfirmed = false // O funcionário deve receber email para ativar/alterar password
                };
                await userHelper.AddUserAsync(user, "Funcionario123!");
                await userHelper.AddUserToRoleAsync(user, "Funcionario");
                // Aqui podes adicionar lógica para enviar email de alteração de password
            }
        }
    }
}
