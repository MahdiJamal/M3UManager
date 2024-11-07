using M3UManager.Helpers;
using System.Text;

namespace M3UManager.Models;

public class Channel : ModelBaseClass
{
    public string MediaUrl { get => _mediaUrl; set => SetProperty(ref _mediaUrl, value); }
    private string _mediaUrl = null;

    public string TvgID { get => _tvgID; set => SetProperty(ref _tvgID, value); }
    private string _tvgID = null;

    public string TvgName { get => _tvgName; set => SetProperty(ref _tvgName, value); }
    private string _tvgName = null;

    public string Logo { get => _tvgLogo; set => SetProperty(ref _tvgLogo, value); }
    private string _tvgLogo = null;
    public string GroupId{ get => _groupId; set => SetProperty(ref _groupId, value); }
    private string _groupId = null;
    public string GroupTitle { get => _groupTitle; set => SetProperty(ref _groupTitle, value); }
    private string _groupTitle = null;

    public string Duration { get => _duration; set => SetProperty(ref _duration, value); }
    private string _duration = null;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    private string _title = null;

    /// <summary>
    /// This method for deserialize <see cref="Channel"/> to <see cref="string"/>.
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
    public static string ChannelToString(Channel channel, M3UType m3uType)
    {
        StringBuilder sb = new();

        sb.Append($"#EXTINF:{channel.Duration}");

        sb.AppendIf(channel.TvgID != null, $" tvg-id=\"{channel.TvgID}\"");

        sb.AppendIf(channel.TvgName != null, $" tvg-name=\"{channel.TvgName}\"");

        if (m3uType == M3UType.TagsType)
        {
            sb.AppendIf(channel.Logo != null, $" tvg-logo=\"{channel.Logo}\"");
            sb.AppendIf(channel.GroupId != null, $" group-id=\"{channel.GroupId}\"");
            sb.AppendIf(channel.GroupTitle != null, $" group-title=\"{channel.GroupTitle}\"");
        }

        sb.Append($",{channel.Title ?? ""}");

        if (m3uType == M3UType.AttributesType)
        {
            sb.AppendIf(channel.GroupTitle != null, $"\r\n#EXTGRP:{channel.GroupTitle}");
            sb.AppendIf(channel.Logo != null, $"\r\n#EXTIMG:{channel.Logo}");
            sb.AppendIf(channel.Title != null, $"\r\n#PLAYLIST:{channel.Title}");
        }

        sb.Append($"\r\n{channel.MediaUrl ?? ""}");

        return sb.ToString();
    }

    /// <inheritdoc cref="ChannelToString(Channel, M3UType)"/>
    public string ToString(M3UType m3uType)
        => ChannelToString(this, m3uType);
}
