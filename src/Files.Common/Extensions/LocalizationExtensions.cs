using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Common.Services;

namespace Files.Common.Extensions
{
    public static class LocalizationExtensions
    {
        private static ILocalizationService localizationService
            = Ioc.Default.GetRequiredService<ILocalizationService>();

        public static string ToLocalized(this string resourceKey)
            => localizationService.LocalizeFromResourceKey(resourceKey);
    }
}
