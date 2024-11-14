using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using testeNav.Models;

namespace testeNav.Services
{
    public class RoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AssignRoleToUser(string email, string roleName)
        {
            // Verifica se o papel existe
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                // Se o papel não existir, cria-o
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Encontra o usuário pelo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Verifica se o usuário já tem o papel
                var isInRole = await _userManager.IsInRoleAsync(user, roleName);
                if (!isInRole)
                {
                    // Adiciona o papel ao usuário
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}