using System;
using System.Collections.Generic;
using System.Globalization;

namespace ShuHai.I18N
{
    public sealed class Language : IEquatable<Language>
    {
        public string Name => CultureInfo.Name;

        public CultureInfo CultureInfo { get; }

        private Language(CultureInfo cultureInfo) { CultureInfo = cultureInfo; }

        #region Texts

        public IReadOnlyCollection<string> InvalidKeys => _invalidKeys;

        public Dictionary<string, string> Texts { get; } = new Dictionary<string, string>();

        public string GetText(string key)
        {
            if (key == null)
                key = string.Empty;

            if (Texts.TryGetValue(key, out var text))
                return text;

            _invalidKeys.Add(key);
            return key;
        }

        private readonly HashSet<string> _invalidKeys = new HashSet<string>();

        #endregion Texts

        #region Equality

        public bool Equals(Language other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(CultureInfo, other.CultureInfo);
        }

        public override bool Equals(object obj) { return obj is Language other && Equals(other); }

        public override int GetHashCode() { return CultureInfo.GetHashCode(); }

        public static bool operator ==(Language l, Language r) { return l?.Equals(r) ?? false; }
        public static bool operator !=(Language l, Language r) { return !(l == r); }

        #endregion Equality

        #region Instances

        public static event Action<Language> Changed;

        public static Language Default { get; }

        public static Language Current
        {
            get => _current;
            set
            {
                var oldLang = _current;
                if (value == _current)
                    return;

                var newLang = value ?? oldLang;
                bool changed = oldLang != newLang;

                _current = newLang;

                if (changed)
                    Changed?.Invoke(oldLang);
            }
        }

        private static Language _current;

        public static IReadOnlyCollection<Language> Instances => _instances.Values;

        public static Language GetOrCreate(string tag)
        {
            Ensure.Argument.NotNullOrEmpty(tag, nameof(tag));
            return GetOrCreate(CultureInfo.GetCultureInfo(tag));
        }

        public static Language GetOrCreate(CultureInfo cultureInfo)
        {
            Ensure.Argument.NotNull(cultureInfo, nameof(cultureInfo));
            return _instances.TryGetValue(cultureInfo.Name, out var lang) ? lang : CreateImpl(cultureInfo);
        }

        public static Language Create(string tag)
        {
            Ensure.Argument.NotNullOrEmpty(tag, nameof(tag));
            return Create(CultureInfo.GetCultureInfo(tag));
        }

        public static Language Create(CultureInfo cultureInfo)
        {
            Ensure.Argument.NotNull(cultureInfo, nameof(cultureInfo));

            var name = cultureInfo.Name;
            if (_instances.ContainsKey(name))
                throw new InvalidOperationException($"Language '{name}' already existed.");
            return CreateImpl(cultureInfo);
        }

        private static readonly Dictionary<string, Language> _instances = new Dictionary<string, Language>();

        private static Language CreateImpl(CultureInfo cultureInfo)
        {
            var lang = new Language(cultureInfo);
            _instances.Add(cultureInfo.Name, lang);
            return lang;
        }

        #endregion Instances

        static Language()
        {
            Default = Create(CultureInfo.CurrentUICulture);
            _current = Default;
        }
    }
}
