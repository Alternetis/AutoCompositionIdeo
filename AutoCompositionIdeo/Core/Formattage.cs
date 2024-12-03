using System.Text;

namespace AutoCompositionIdeo.Core
{
    class Formattage
    {
        #region Suppresion, Mise en forme du text, formattage
        public static string EscapeSqlString(string value)
        {
            return value.Replace("'", "''");
        }
        public static string RemovePurge(string input, int length_limit)
        {
            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace("<", string.Empty);
            builder = builder.Replace(">", string.Empty);
            builder = builder.Replace(";", string.Empty);
            builder = builder.Replace("=", string.Empty);
            builder = builder.Replace("#", string.Empty);
            builder = builder.Replace("{", string.Empty);
            builder = builder.Replace("}", string.Empty);

            if (builder.Length > length_limit)
            {
                builder.Remove(length_limit - 1, builder.Length - length_limit);
            }

            return builder.ToString();
        }

        public static string RemovePurgeMeta(string input, int length_limit)
        {
            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace("<", string.Empty);
            builder = builder.Replace(">", string.Empty);
            builder = builder.Replace(";", string.Empty);
            builder = builder.Replace("=", string.Empty);
            builder = builder.Replace("#", string.Empty);
            builder = builder.Replace("{", string.Empty);
            builder = builder.Replace("}", string.Empty);

            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("+", string.Empty);
            builder = builder.Replace("\"", string.Empty);
            builder = builder.Replace("°", string.Empty);
            builder = builder.Replace("_", string.Empty);
            builder = builder.Replace("$", string.Empty);
            builder = builder.Replace("%", string.Empty);

            if (builder.Length > length_limit)
            {
                builder.Remove(length_limit - 1, builder.Length - length_limit);
            }

            return builder.ToString();
        }

        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            else
            {
                string formD = input.Normalize(NormalizationForm.FormD);
                StringBuilder sbNoDiacritics = new StringBuilder();
                foreach (char c in formD)
                {
                    if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                        sbNoDiacritics.Append(c);
                }
                string noDiacritics = sbNoDiacritics.ToString().Normalize(NormalizationForm.FormC);
                return noDiacritics;
            }
        }

        public static string ReadLinkRewrite(string input)
        {
            input = RemoveDiacritics(input);
            input = RemovePurge(input, input.Length + 1);

            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace(", ", "-");
            builder = builder.Replace("\\", string.Empty);
            builder = builder.Replace("/", string.Empty);
            builder = builder.Replace("(", string.Empty);
            builder = builder.Replace(")", string.Empty);
            // ASCII character 32
            builder = builder.Replace(" ", "-");
            // ASCII character 160
            builder = builder.Replace(" ", "-");
            builder = builder.Replace(",", "-");
            builder = builder.Replace("+", string.Empty);
            builder = builder.Replace("'", string.Empty);
            builder = builder.Replace("&", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("^", string.Empty);
            builder = builder.Replace("$", string.Empty);
            builder = builder.Replace("£", string.Empty);
            builder = builder.Replace("*", string.Empty);
            builder = builder.Replace(":", string.Empty);
            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("@", "a");
            builder = builder.Replace("\"", string.Empty);
            builder = builder.Replace(".", string.Empty);
            builder = builder.Replace("§", string.Empty);
            builder = builder.Replace("°", string.Empty);
            builder = builder.Replace("¨", string.Empty);
            builder = builder.Replace("%", string.Empty);
            builder = builder.Replace("µ", string.Empty);
            builder = builder.Replace("€", string.Empty);
            builder = builder.Replace("²", "2");
            builder = builder.Replace("Ø", "d");
            builder = builder.Replace("©", string.Empty);
            builder = builder.Replace("®", string.Empty);
            builder = builder.Replace("~", string.Empty);
            builder = builder.Replace("¼", string.Empty);
            builder = builder.Replace("½", string.Empty);
            builder = builder.Replace("¾", string.Empty);
            builder = builder.Replace("¦", string.Empty);
            builder = builder.Replace("|", string.Empty);
            builder = builder.Replace("»", string.Empty);
            builder = builder.Replace("’", "-");
            builder = builder.Replace("«", string.Empty);

            //builder = builder.Replace("", string.Empty);

            while (builder.ToString().IndexOf("--") != -1)
                builder = builder.Replace("--", "-");

            while (builder.ToString().EndsWith("-"))
                builder = builder.Remove(builder.ToString().LastIndexOf("-"), 1);

            string output = builder.ToString().ToLower();
            if (output.Length > 128)
                output = output.Substring(0, 128);

            return output;
        }
        #endregion
    }
}
