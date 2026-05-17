using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NeoIniLight.Models;
using Data = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace NeoIniLight.Core
{
    internal partial class NeoIniParser
    {
#if NETSTANDARD2_0
        internal static string FormatInvariant<T>(T value)
#else
        internal static string FormatInvariant<T>(T? value)
#endif
        {
            if (value is DateTime dt)
                return dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            if (value is DateTimeOffset dto)
                return dto.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            return value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value?.ToString() ?? string.Empty;
        }

#if NETSTANDARD2_0
        internal static string ValueToString<T>(T value)
#else
        internal static string ValueToString<T>(T? value)
#endif
        {
            var s = FormatInvariant(value);
            if (s.Length == 0) return s;
            var sb = new StringBuilder(s.Length * 2);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\r')
                {
                    if (i + 1 < s.Length && s[i + 1] == '\n')
                    {
                        sb.Append(@"\r\n");
                        i++;
                    }
                    else sb.Append(@"\r");
                    continue;
                }
                if (c == '\n') { sb.Append(@"\n"); continue; }
                if (c == '\\') { sb.Append(@"\\"); continue; }
                sb.Append(c);
            }
            return FormatInvariant(sb);
        }

        internal static string? GetStringRaw(string? raw) => Unescape(raw);


        internal static string? GetStringRaw(Data? data, string section, string keyName)
        {
            string? raw = null;
#if NETSTANDARD2_0
            if (!(data is null) && data.TryGetValue(section, out var sec) && sec.TryGetValue(keyName, out var val))
#else
            if (data is not null && data.TryGetValue(section, out var sec) && sec.TryGetValue(keyName, out var val))
#endif
                raw = val.Trim();
            return Unescape(raw);
        }

#if NETSTANDARD2_0
        internal static T TryParseValue<T>(string? value, T defaultValue, EventHandler<ProviderErrorEventArgs>? onError)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            try
            {
                if (targetType.IsEnum)
                {
                    try { return (T)Enum.Parse(targetType, value, true); }
                    catch { return defaultValue; }
                }
                if (targetType == typeof(bool))
                    return bool.TryParse(value, out bool boolResult) ? (T)(object)boolResult : defaultValue;
                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                                out DateTime dtResult))
                        return (T)(object)dtResult;
                    if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dtResult))
                        return (T)(object)dtResult;
                    return defaultValue;
                }
                return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                onError?.Invoke(null, new ProviderErrorEventArgs(ex));
                return defaultValue;
            }
            catch (InvalidCastException ex)
            {
                onError?.Invoke(null, new ProviderErrorEventArgs(ex));
                return defaultValue;
            }
        }
#else
        internal static T? TryParseValue<T>(string? value, T? defaultValue, EventHandler<ProviderErrorEventArgs>? onError)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            try
            {
                if (targetType.IsEnum)
                    return Enum.TryParse(targetType, value, true, out object? enumResult) && enumResult is not null ? (T)enumResult : defaultValue;
                if (targetType == typeof(bool))
                    return bool.TryParse(value, out bool boolResult) ? (T)(object)boolResult : defaultValue;
                if (targetType == typeof(DateTime))
                {
                    if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                                out DateTime dtResult))
                        return (T)(object)dtResult;
                    if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dtResult))
                        return (T)(object)dtResult;
                    return defaultValue;
                }
                return (T?)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                onError?.Invoke(null, new ProviderErrorEventArgs(ex));
                return defaultValue;
            }
            catch (InvalidCastException ex)
            {
                onError?.Invoke(null, new ProviderErrorEventArgs(ex));
                return defaultValue;
            }
        }
#endif

#if NETSTANDARD2_0
        internal static bool TryMatchKey(string line, out string? key, out string? value)
        {
            key = null;
            value = null;
            int eqIndex = line.IndexOf('=');
            if (eqIndex == -1) return false;
            string keyStr = line.Substring(0, eqIndex).Trim();
            if (string.IsNullOrEmpty(keyStr)) return false;
            key = keyStr;
            value = line.Substring(eqIndex + 1).Trim();
            return true;
        }
#else
        internal static bool TryMatchKey(ReadOnlySpan<char> line, out string? key, out string? value)
        {
            key = null;
            value = null;
            int eqIndex = line.IndexOf('=');
            if (eqIndex == -1) return false;
            ReadOnlySpan<char> keySpan = line[..eqIndex].Trim();
            if (keySpan.IsEmpty) return false;
            ReadOnlySpan<char> valueSpan = line[(eqIndex + 1)..].Trim();
            key = keySpan.ToString();
            value = valueSpan.ToString();
            return true;
        }
#endif

#if NETSTANDARD2_0
        internal static T Clamp<T>(T value, T minValue, T maxValue) where T : IComparable<T>
#else
        internal static T Clamp<T>(T? value, T minValue, T maxValue) where T : IComparable<T>
#endif
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(minValue, maxValue) > 0)
                throw new ArgumentException($"'{nameof(minValue)}' cannot be greater than '{nameof(maxValue)}'.");
            if (value is null) return minValue;
            if (comparer.Compare(value, minValue) < 0) return minValue;
            if (comparer.Compare(value, maxValue) > 0) return maxValue;
            return value;
        }

        internal static string GetContent(Data? data)
        {
            if (data is null || data.Count == 0) return string.Empty;
            int estimatedSize = Environment.NewLine.Length;
            foreach (var section in data)
            {
                estimatedSize += section.Key.Length + 2 + Environment.NewLine.Length;
                foreach (var kvp in section.Value)
                    estimatedSize += kvp.Key.Length + (kvp.Value?.Length ?? 0) + 3 + Environment.NewLine.Length;
                estimatedSize += Environment.NewLine.Length;
            }
            var content = new StringBuilder(estimatedSize);
            content.AppendLine();
            foreach (var section in data)
            {
                content.AppendLine($"[{section.Key}]");
                foreach (var kvp in section.Value)
                    content.AppendLine($"{kvp.Key} = {kvp.Value}");
                content.AppendLine();
            }
            return content.ToString();
        }

        internal static bool IsSectionLine(string? trimmed)
        {
#if NETSTANDARD2_0
            if (trimmed is null) return false;
#else
            if (string.IsNullOrEmpty(trimmed)) return false;
#endif
            return trimmed.Length > 1 && trimmed[0] == '[' && trimmed[trimmed.Length - 1] == ']';
        }

        internal static string? HandleSectionLine(string? trimmed, Data data)
        {
#if NETSTANDARD2_0
            if (trimmed is null) return trimmed;
#else
            if (string.IsNullOrEmpty(trimmed)) return trimmed;
#endif
            var section = trimmed.Trim('[', ']');
            if (!data.ContainsKey(section)) data[section] = new Dictionary<string, string>();
            return section;
        }

        internal static void HandleKeyValueLine(string? trimmed, string? currentSection, string? key, string? value, Data? data)
        {
            if (data is null) return;
#if NETSTANDARD2_0
            if (currentSection is null || key is null) return;
#else
            if (string.IsNullOrEmpty(currentSection) || string.IsNullOrEmpty(key)) return;
#endif
            if (!data.ContainsKey(currentSection)) data[currentSection] = new Dictionary<string, string>();
            data[currentSection][key] = value ?? string.Empty;
        }
    }
}
