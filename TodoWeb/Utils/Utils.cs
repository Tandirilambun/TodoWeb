using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TodoWeb.Data;
using TodoWeb.Models;

namespace TodoWeb.Utils
{
    public static class Utils
    {
        public static async Task<Category> GetCategoryAsync(ApplicationDbContext context, int id)
        {
            return await context.Category
                .Where(cat => cat.Category_id == id)
                .FirstOrDefaultAsync();
        }

        public static string GenerateKey()
        {
            using ( var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[32];
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }
    }
}
