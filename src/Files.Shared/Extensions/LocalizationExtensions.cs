using Windows.ApplicationModel.Resources;

namespace Files.Shared.Extensions
{
    public static class LocalizationExtensions
    {
        private static readonly ResourceLoader loader = ResourceLoader.GetForCurrentView("Files.Shared");

        public static string GetLocalized(this string resourceKey) => loader.GetString(resourceKey);
    }
}
