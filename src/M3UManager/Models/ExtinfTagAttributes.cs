using System;
using System.Text.RegularExpressions;

namespace M3UManager.Models
{
    public class ExtinfTagAttributes
    {
        public string TvgID { get; set; }
        public string TvgName { get; set; }
        public string TvgLogo { get; set; }
        public string GroupTitle { get; set; }
        public string Duration { get; set; }
        public string Title { get; set; }

        public static ExtinfTagAttributes Parse(string extinfTagAttributesWithoutTagName)
        {
            try
            {
                return new ExtinfTagAttributes()
                {
                    Duration = extinfTagAttributesWithoutTagName.Split(' ')[0],
                    TvgID = Regex.Match(extinfTagAttributesWithoutTagName, "tvg-id=\"(.*?)\"", RegexOptions.IgnoreCase).Groups[1].Value,
                    TvgName = Regex.Match(extinfTagAttributesWithoutTagName, "tvg-name=\"(.*?)\"", RegexOptions.IgnoreCase).Groups[1].Value,
                    TvgLogo = Regex.Match(extinfTagAttributesWithoutTagName, "tvg-logo=\"(.*?)\"", RegexOptions.IgnoreCase).Groups[1].Value,
                    GroupTitle = Regex.Match(extinfTagAttributesWithoutTagName, "group-title=\"(.*?)\"", RegexOptions.IgnoreCase).Groups[1].Value,
                    Title = extinfTagAttributesWithoutTagName.Split(',')[1]
                };
            }
            catch (Exception e)
            {
                throw new Exception($"{extinfTagAttributesWithoutTagName}\r\n\r\n{e.Message}");
            }
        }

        private const string ExtinfTagAttributesFormat = @"{0} tvg-id=""{1}"" tvg-name=""{2}"" tvg-logo=""{3}"" group-title=""{4}"",{5}";

        public override string ToString()
            => string.Format(ExtinfTagAttributesFormat, Duration, TvgID, TvgName, TvgLogo, GroupTitle, Title);
    }
}
