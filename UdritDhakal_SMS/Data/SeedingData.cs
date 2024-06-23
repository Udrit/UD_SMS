using Microsoft.AspNetCore.Identity;

namespace UdritDhakal_SMS.Data
{
    public class SeedingData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] Roles = { "ADMIN", "STUDENT" };
            foreach (string roleName in Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
