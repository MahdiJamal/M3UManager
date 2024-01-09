using System;
using System.Text.RegularExpressions;

namespace M3UManager.Models;

public partial class ExtinfTagAttributes: NotifyPropertyChanged
{
    private string _tvgID = null;
    public string TvgID { get => _tvgID; set => SetProperty(ref _tvgID, value); }

    private string _tvgName = null;
    public string TvgName { get => _tvgName; set => SetProperty(ref _tvgName, value); }

    private string _tvgLogo = null;
    public string TvgLogo { get => _tvgLogo; set => SetProperty(ref _tvgLogo, value); }

    private string _groupTitle = null;
    public string GroupTitle { get => _groupTitle; set => SetProperty(ref _groupTitle, value); }

    private string _duration = null;
    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }

    private string _title = null;
    public string Title { get => _title; set => SetProperty(ref _title, value); }


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
