using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentity.Model;
using Amazon.Extensions.CognitoAuthentication;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(CognitoUserManager<CognitoUser> userManager, CognitoRoleStore<CognitoRole> roleStore)
        {
            var administratorRole = new CognitoRole("Administrators", "Administrator", 0);
            var role = await roleStore.FindByNameAsync(administratorRole.Name, CancellationToken.None);
            if (role == null)
            {
                await roleStore.CreateAsync(administratorRole, CancellationToken.None);
            }

            var administratorUser = new ApplicationUser ("administrator@localhost");
            var user = await userManager.FindByNameAsync(administratorUser.Username);
            if (user == null)
            {
                await userManager.CreateAsync(administratorUser, "Administrator1!");
                await userManager.AddToRolesAsync(administratorUser, new[] { administratorRole.Name });
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(new TodoList
                {
                    Title = "Shopping",
                    Colour = Colour.Blue,
                    Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" },
                        new TodoItem { Title = "Water" }
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
