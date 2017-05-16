using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Text;

namespace DiacriticRemoverDotNext
{
    static class StringExtensions
    {
        // stackoverflow http://stackoverflow.com/questions/1271567/how-do-i-replace-accents-german-in-net
        private static Dictionary<char, string> map = new Dictionary<char, string>() {
              { 'ä', "ae" },
              { 'ö', "oe" },
              { 'ü', "ue" },
              { 'Ä', "Ae" },
              { 'Ö', "Oe" },
              { 'Ü', "Ue" },
              { 'ß', "ss" }
            };

        internal static string RemoveGermanDiactrics(this string germanText)
        {
            return germanText.ToCharArray().Aggregate(new StringBuilder(),
                          (sb, c) =>
                          {
                              if (map.TryGetValue(c, out string r))
                                  return sb.Append(r);
                              else
                                  return sb.Append(c);
                          }).ToString();
        }

        internal static bool ContainsGermanDiactrics(this string text)
        {
            return text.ToCharArray().Any(x => map.ContainsKey(x));
        }
    }
}