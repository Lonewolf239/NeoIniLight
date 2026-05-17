using System.Text;

namespace NeoIniLight.Core
{
    internal partial class NeoIniParser
    {
        private static string? Unescape(string? s)
        {
#if NETSTANDARD2_0
            if (s is null) return s;
#else
            if (string.IsNullOrEmpty(s)) return s;
#endif
            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\\' && i + 1 < s.Length)
                {
                    char next = s[i + 1];
                    switch (next)
                    {
                        case 'r':
                            if (i + 3 < s.Length && s[i + 2] == '\\' && s[i + 3] == 'n')
                            {
                                sb.Append('\r').Append('\n');
                                i += 3;
                            }
                            else
                            {
                                sb.Append('\r');
                                i++;
                            }
                            continue;
                        case 'n': sb.Append('\n'); i++; continue;
                        case '\\': sb.Append('\\'); i++; continue;
                        case '"': sb.Append('"'); i++; continue;
                    }
                }
                sb.Append(c);
            }
            return FormatInvariant(sb);
        }
    }
}
