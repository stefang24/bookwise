using System.Text;

namespace backend.Helpers
{
    public static class ConfigProvider
    {
        public static string JwtSecret { get; private set; } = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            JwtSecret = configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
        }

        public static byte[] GetJwtKeyBytes()
        {
            return Encoding.UTF8.GetBytes(JwtSecret);
        }
    }
}
