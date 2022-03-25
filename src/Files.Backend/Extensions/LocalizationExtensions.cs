using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Backend.Services;
using System;
using System.Collections.Generic;

#nullable enable

namespace Files.Backend.Extensions
{
    public static class LocalizationExtensions
    {
        private static ILocalizationService? FallbackLocalizationService;

        public static string ToLocalized(this string resourceKey, ILocalizationService? localizationService = null)
        {
            if (localizationService == null)
            {
                FallbackLocalizationService ??= Ioc.Default.GetService<ILocalizationService>();
                return FallbackLocalizationService?.LocalizeFromResourceKey(resourceKey) ?? string.Empty;
            }

            return localizationService.LocalizeFromResourceKey(resourceKey);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T value in collection)
                action(value);
        }
    }
}
