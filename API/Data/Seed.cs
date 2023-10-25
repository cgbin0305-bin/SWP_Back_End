using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task UserSeedData(WebContext context)
        {
            if (await context.Users.AnyAsync()) return;
            var userData = await File.ReadAllTextAsync("Data/SeedData/User.json");
            var choresData = await File.ReadAllTextAsync("Data/SeedData/HouseHoldChores.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<User>>(userData, options); // let JSON into C-sharp object
            var chores = JsonSerializer.Deserialize<List<HouseHoldChores>>(choresData, options);
            foreach (var chore in chores)
            {
                context.HouseHoldChores.Add(chore);
            }
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.Name = user.Name.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                user.PasswordSalt = hmac.Key;
                if (user.Role.ToLower() == "worker")
                {
                    user.Worker.Id = user.Id;
                }
                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}