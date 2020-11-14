using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShuHai
{
    /// <summary>
    ///     Extra methods of <see cref="Path" />.
    /// </summary>
    public static class PathEx
    {
#if UNITY_5_3_OR_NEWER
        public const char DirectorySeparatorChar = '/';
        public const char AltDirectorySeparatorChar = '\\';
#else
        public const char DirectorySeparatorChar = Path.DirectorySeparatorChar;
        public const char AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
#endif

        /// <summary>
        ///     Convert specified slash charactor to its opposite slash charactor.
        /// </summary>
        public static char AltSeparatorSlashOf(char slash)
        {
            switch (slash)
            {
                case '\\':
                    return '/';
                case '/':
                    return '\\';
                default:
                    throw new ArgumentOutOfRangeException(nameof(slash));
            }
        }

        public static string Normalize(string path) { return Normalize(path, DirectorySeparatorChar); }

        /// <summary>
        ///     Convert all specified directory separate slash to its opposite slash and remove all redundant slashes.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <param name="separatorSlash">Slash character to apply to the path.</param>
        /// <returns>A normalized path with all directory separate slash converted to <paramref name="separatorSlash" />.</returns>
        public static string Normalize(string path, char separatorSlash)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            var altSlash = AltSeparatorSlashOf(separatorSlash);
            var sb = new StringBuilder(path.Length);
            var len = path.Length;
            for (var i = 0; i < len; ++i)
            {
                var c = path[i];
                if (c == altSlash)
                    c = separatorSlash;
                var sbl = sb.Length;
                if (c == separatorSlash && sbl > 0 && sb[sbl - 1] == separatorSlash)
                    continue;
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Similar to <see cref="System.IO.Path.Combine(string,string)" /> but returns normalized path.
        /// </summary>
        public static string Combine(string path1, string path2) { return Normalize(Path.Combine(path1, path2)); }

        /// <summary>
        ///     Similar to <see cref="System.IO.Path.Combine(string,string,string)" /> but returns normalized path.
        /// </summary>
        public static string Combine(string path1, string path2, string path3)
        {
            return Normalize(Path.Combine(path1, path2, path3));
        }

        /// <summary>
        ///     Similar to <see cref="System.IO.Path.Combine(string,string,string,string)" /> but returns normalized path.
        /// </summary>
        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return Normalize(Path.Combine(path1, path2, path3, path4));
        }

        /// <summary>
        ///     Similar to <see cref="System.IO.Path.Combine(string[])" /> but returns normalized path.
        /// </summary>
        public static string Combine(params string[] path) { return Normalize(Path.Combine(path)); }

        /// <summary>
        ///     Get the filename or directory name of the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the name.</param>
        public static string NameOf(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            var trimmedPath = path.Trim(_separators);
            var index = trimmedPath.LastIndexOfAny(_separators);
            if (index < 0)
            {
                if (trimmedPath.Length > 0 && trimmedPath[trimmedPath.Length - 1] == Path.VolumeSeparatorChar)
                    return string.Empty;
                return trimmedPath;
            }

            return trimmedPath.Substring(index + 1);
        }

        /// <summary>
        ///     Get the parent directory name of the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the name.</param>
        public static string ParentOf(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            var index = path.Trim(_separators).LastIndexOfAny(_separators);
            if (index >= 0)
            {
                if (index > 0 && path[index - 1] == Path.VolumeSeparatorChar)
                    index--;
                return path.Remove(index);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>
        ///     The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string MakeRelative(string fromPath, string toPath)
        {
            Ensure.Argument.NotNullOrEmpty(fromPath, nameof(fromPath));
            Ensure.Argument.NotNullOrEmpty(toPath, nameof(toPath));

            Uri fromUri = new Uri(fromPath), toUri = new Uri(toPath);
            if (fromUri.Scheme != toUri.Scheme)
                throw new ArgumentException("Can't make relative path for path of different types.");

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
                relativePath = relativePath.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);

            return relativePath;
        }

        public static bool AreEqual(string path1, string path2)
        {
            bool rooted1 = Path.IsPathRooted(path1), rooted2 = Path.IsPathRooted(path2);
            if (rooted1 && rooted2)
                return Path.GetFullPath(path1) == Path.GetFullPath(path2);
            if (rooted1 || rooted2)
                return false;
            return Normalize(path1) == Normalize(path2);
        }

        /// <summary>
        ///     Split specified path into folder and file names from which made the path.
        /// </summary>
        /// <param name="path">The path string to split.</param>
        public static IEnumerable<string> Split(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            var names = path.Split(_separators);
            return names.Where(n => n != string.Empty);
        }

        private static readonly char[] _separators = { DirectorySeparatorChar, AltDirectorySeparatorChar };
    }
}