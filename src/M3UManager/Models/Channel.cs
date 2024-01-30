using System;
using System.Text;

namespace M3UManager.Models;

public partial class Channel : NotifyPropertyChanged, ICloneable
{
    public string MediaUrl { get => _mediaUrl; set => SetProperty(ref _mediaUrl, value); }
    private string _mediaUrl = null;

    public string TvgID { get => _tvgID; set => SetProperty(ref _tvgID, value); }
    private string _tvgID = null;

    public string TvgName { get => _tvgName; set => SetProperty(ref _tvgName, value); }
    private string _tvgName = null;

    public string Logo { get => _tvgLogo; set => SetProperty(ref _tvgLogo, value); }
    private string _tvgLogo = null;

    public string GroupTitle { get => _groupTitle; set => SetProperty(ref _groupTitle, value); }
    private string _groupTitle = null;

    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }
    private string _duration = null;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    private string _title = null;

    /// <summary>
    /// This method for serialize <see cref="Channel"/> to <see cref="string"/>.
    /// </summary>
    /// <returns>
    /// <see cref="M3UType.AttributesType"/> example:
    /// <example>
    /// <br/>
    /// #EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="Undefined",HDTV (720p) [Not 24/7]
    /// <br/>
    /// http://0.0.0.0/hbbh/stream.m3u8
    /// <br/>
    /// <br/>
    /// </example>
    /// <see cref="M3UType.TagsType"/> example:
    /// <example>
    /// <br/>
    /// #EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png",HDTV (720p) [Not 24/7]
    /// <br/>
    /// http://0.0.0.0/hbbh/stream.m3u8
    /// <br/>
    /// #EXTGRP:Undefined
    /// </example>
    /// </returns>
    public string ChannelToString(M3UType m3uType)
    {
        StringBuilder sb = new();

        sb.Append($"#EXTINF:{Duration}");

        if (TvgID != null)
            sb.Append($" tvg-id=\"{TvgID}\"");

        if (TvgName != null)
            sb.Append($" tvg-name=\"{TvgName}\"");

        if (m3uType == M3UType.TagsType)
        {
            if (Logo != null)
                sb.Append($" tvg-logo=\"{Logo}\"");

            if (GroupTitle != null)
                sb.Append($" group-title=\"{GroupTitle}\"");
        }

        sb.Append($",{Title ?? ""}");

        if (m3uType == M3UType.AttributesType)
        {
            if (GroupTitle != null)
                sb.Append($"\r\n#EXTGRP:{GroupTitle}");

            if (Logo != null)
                sb.Append($"\r\n#EXTIMG:{Logo}");

            if (Title != null)
                sb.Append($"\r\n#PLAYLIST:{Title}");
        }

        sb.Append($"\r\n{MediaUrl ?? ""}");

        return sb.ToString();
    }

    public object Clone()
        => MemberwiseClone();
    public T Clone<T>()
        => (T)Clone();

    public void Reset2()
    {
        MediaUrl = null;
        TvgID = null;
        TvgName = null;
        Logo = null;
        GroupTitle = null;
        Duration = null;
        Title = null;
    }
}
