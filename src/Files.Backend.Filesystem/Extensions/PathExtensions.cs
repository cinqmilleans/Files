using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Shared;
using Files.Shared.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Files.Backend.Filesystem.Extensions
{
    public static class PathExtensions
    {
        private static readonly ILogger logger = Ioc.Default.GetService<ILogger>();

        public static string GetPathRoot(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            string rootPath = string.Empty;
            try
            {
                var pathAsUri = new Uri(path.Replace('\\', '/'));
                rootPath = pathAsUri.GetLeftPart(UriPartial.Authority);
                if (pathAsUri.IsFile && !string.IsNullOrEmpty(rootPath))
                {
                    rootPath = new Uri(rootPath).LocalPath;
                }
            }
            catch (UriFormatException)
            {
            }
            return string.IsNullOrEmpty(rootPath) ? Path.GetPathRoot(path) : rootPath;
        }

        public static string GetParentDir(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            if (path.Contains('/'))
            {
                return path.Left(path.LastIndexOf('/'));
            }
            if (path.Contains('\\'))
            {
                return path.Left(path.LastIndexOf('\\'));
            }
            return path;
        }

        public static string CombinePath(this string folder, string name)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return name;
            }
            return folder.Contains('/')
                ? Path.Combine(folder, name).Replace('\\', '/')
                : Path.Combine(folder, name);
        }

        public static string TrimPath(this string path) => path?.TrimEnd('\\', '/');

        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            if (path.StartsWith("\\\\") || path.StartsWith("//") || FtpHelpers.IsFtpPath(path))
            {
                return path.TrimEnd('\\', '/').ToUpperInvariant();
            }
            if (!path.EndsWith(Path.DirectorySeparatorChar))
            {
                path += Path.DirectorySeparatorChar;
            }

            try
            {
                return Path.GetFullPath(new Uri(path).LocalPath)
                    .TrimEnd('\\', '/')
                    .ToUpperInvariant();
            }
            catch (UriFormatException ex)
            {
                logger?.Warn(ex, path);
                return path;
            }
        }
    }
}
