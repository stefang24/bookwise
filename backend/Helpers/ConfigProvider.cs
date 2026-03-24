using System.Text;

namespace backend.Helpers
{
    public static class ConfigProvider
    {
        private static IConfiguration? _configuration;

        public static string JwtSecret { get; private set; } = string.Empty;
        public static string DefaultProfileImagePath { get; private set; } = string.Empty;
        public static string DefaultServiceImagePath { get; private set; } = string.Empty;
        public static long MaxImageFileSizeBytes { get; private set; }
        public static IReadOnlySet<string> AllowedImageExtensions { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static string AllowedImageExtensionsText { get; private set; } = string.Empty;
        public static string ProfileImagesFolderRelativePath { get; private set; } = string.Empty;
        public static string ServiceImagesFolderRelativePath { get; private set; } = string.Empty;
        public static string ProfileImageFilePrefix { get; private set; } = string.Empty;
        public static string ServiceImageFilePrefix { get; private set; } = string.Empty;
        public static IReadOnlyList<string> ProviderPredefinedCategories { get; private set; } = [];

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
            JwtSecret = configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

            DefaultProfileImagePath = GetRequiredString("AppConfig:Defaults:ProfileImagePath");
            DefaultServiceImagePath = GetRequiredString("AppConfig:Defaults:ServiceImagePath");

            MaxImageFileSizeBytes = configuration.GetValue<long?>("AppConfig:Uploads:MaxImageFileSizeBytes")
                ?? throw new InvalidOperationException("AppConfig:Uploads:MaxImageFileSizeBytes is not configured.");

            string[] configuredExtensions = configuration.GetSection("AppConfig:Uploads:AllowedImageExtensions").Get<string[]>()
                ?? throw new InvalidOperationException("AppConfig:Uploads:AllowedImageExtensions is not configured.");

            string[] normalizedExtensions = configuredExtensions
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLowerInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (normalizedExtensions.Length == 0)
                throw new InvalidOperationException("AppConfig:Uploads:AllowedImageExtensions must not be empty.");

            AllowedImageExtensions = new HashSet<string>(normalizedExtensions, StringComparer.OrdinalIgnoreCase);
            AllowedImageExtensionsText = string.Join(", ", normalizedExtensions);

            ProfileImagesFolderRelativePath = NormalizeRelativePath(GetRequiredString("AppConfig:Uploads:ProfileImagesFolder"));
            ServiceImagesFolderRelativePath = NormalizeRelativePath(GetRequiredString("AppConfig:Uploads:ServiceImagesFolder"));
            ProfileImageFilePrefix = GetRequiredString("AppConfig:Uploads:ProfileImageFilePrefix");
            ServiceImageFilePrefix = GetRequiredString("AppConfig:Uploads:ServiceImageFilePrefix");

            string[] categories = configuration.GetSection("AppConfig:ProviderCatalog:PredefinedCategories").Get<string[]>()
                ?? throw new InvalidOperationException("AppConfig:ProviderCatalog:PredefinedCategories is not configured.");

            ProviderPredefinedCategories = categories
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public static byte[] GetJwtKeyBytes()
        {
            return Encoding.UTF8.GetBytes(JwtSecret);
        }

        private static string GetRequiredString(string key)
        {
            if (_configuration == null)
                throw new InvalidOperationException("ConfigProvider.Initialize must be called before reading configuration values.");

            string? value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{key} is not configured.");

            return value.Trim();
        }

        private static string NormalizeRelativePath(string path)
        {
            return path.Trim().TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
