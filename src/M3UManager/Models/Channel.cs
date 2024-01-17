using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace M3UManager.Models;

public partial class Channel : NotifyPropertyChanged, ICloneable
{
    public string MediaUrl { get => _mediaUrl; set => SetProperty(ref _mediaUrl, value); }
    private string _mediaUrl = null;

    public string TvgID { get => _tvgID; set => SetProperty(ref _tvgID, value); }
    private string _tvgID = null;

    public string TvgName { get => _tvgName; set => SetProperty(ref _tvgName, value); }
    private string _tvgName = null;

    public string TvgLogo { get => _tvgLogo; set => SetProperty(ref _tvgLogo, value); }
    private string _tvgLogo = null;

    public string GroupTitle { get => _groupTitle; set => SetProperty(ref _groupTitle, value); }
    private string _groupTitle = null;

    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }
    private string _duration = null;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    private string _title = null;

    private static Dictionary<string, string> ExtractExtinfAttributes(string extinfTagAttributesWithoutTagName)
    {
        const string pattern = @"(?<=\s|\n|^)([a-z\-]+)=\""(.+?)\""";

        return Regex.Matches(extinfTagAttributesWithoutTagName, pattern, RegexOptions.IgnoreCase)
                                .Cast<Match>()
                                .ToDictionary(match => match.Groups[0].Value, match => match.Groups[1].Value);
    }

    public void DetectAndSetExtinfProperties(string extinfTagAttributesWithoutTagName)
    {
        var attributes = ExtractExtinfAttributes(extinfTagAttributesWithoutTagName);

        TvgID = attributes.ContainsKey("tvg-id") ? attributes["tvg-id"] : null;
        TvgName = attributes.ContainsKey("tvg-name") ? attributes["tvg-name"] : null;
        TvgLogo = attributes.ContainsKey("tvg-logo") ? attributes["tvg-logo"] : null;
        GroupTitle = attributes.ContainsKey("group-title") ? attributes["group-title"] : null;
    }

    /// <summary>
    /// This channel to string.
    /// </summary>
    /// <returns>
    /// InlineGroupTitle example:
    /// <br/>
    /// #EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="Undefined",HDTV (720p) [Not 24/7]
    /// <br/>
    /// http://0.0.0.0/hbbh/stream.m3u8
    /// <br/>
    /// <br/>
    /// OutlineGroupTitle example:
    /// <br/>
    /// #EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png",HDTV (720p) [Not 24/7]
    /// <br/>
    /// http://0.0.0.0/hbbh/stream.m3u8
    /// <br/>
    /// #EXTGRP:Undefined
    /// </returns>
    public string ChannelToString(M3UGroupTitle m3uGroupTitle)
    {
        StringBuilder sb = new();

        sb.Append($"#EXTINF:{Duration}");

        if (TvgID != null)
            sb.Append($" tvg-id=\"{TvgID}\"");

        if (TvgName != null)
            sb.Append($" tvg-name=\"{TvgName}\"");

        if (TvgLogo != null)
            sb.Append($" tvg-logo=\"{TvgLogo}\"");

        if (GroupTitle != null && m3uGroupTitle == M3UGroupTitle.InlineGroupTitle)
            sb.Append($" group-title=\"{GroupTitle}\"");

        sb.Append($",{Title ?? ""}");

        if (GroupTitle != null && m3uGroupTitle == M3UGroupTitle.OutlineGroupTitle)
            sb.Append($"\r\n#EXTGRP:{GroupTitle}");

        sb.Append($"\r\n{MediaUrl ?? ""}");

        return sb.ToString();
    }

    public object Clone()
    => MemberwiseClone();
    public T Clone<T>()
        => (T)Clone();

    public void Reset()
    {
        MediaUrl = null;
        TvgID = null;
        TvgName = null;
        TvgLogo = null;
        GroupTitle = null;
        Duration = null;
        Title = null;
    }
}








//public partial class Channel : NotifyPropertyChanged, ICloneable
//{
//    private string _mediaUrl = null;
//    public string MediaUrl { get => _mediaUrl; set => SetProperty(ref _mediaUrl, value); }

//    private ExtinfTag _extinfTag = null;
//    public ExtinfTag ExtinfTag { get => _extinfTag; set => SetProperty(ref _extinfTag, value); }

//    public object Clone()
//        => MemberwiseClone();
//    public T Clone<T>()
//        => (T)Clone();

//    public void Reset()
//    {
//        MediaUrl = null;
//        ExtinfTag = null;
//    }
//}
