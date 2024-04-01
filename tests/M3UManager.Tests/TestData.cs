using M3UManager.Models;

namespace M3UManager.Tests;

public class TestData
{
    //********** Attributes extinf **********//

    public const string SampleAttributesExtinfContent = """
#EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://y.imyr.cm/xy70wD.png" group-title="",HDTV (720p)
http://0.0.0.0/hbbh/stream.m3u8
""";

    public static readonly Stack<string> SampleAttributesExtinfLines
        = new(SampleAttributesExtinfContent.Split(["\r\n", "\n", "\r"], StringSplitOptions.None));

    public static readonly Channel SampleAttributesExtinfChannel = new()
    {
        Title = "HDTV (720p)",
        Duration = "-1",
        GroupTitle = "",
        TvgID = "HDTV.fr",
        Logo = "https://y.imyr.cm/xy70wD.png",
        TvgName = null,
        MediaUrl = "http://0.0.0.0/hbbh/stream.m3u8",
    };

    //********** Tags extinf **********//

    public const string SampleTagsExtinfContent = """
#EXTINF:-1 tvg-id="HDTV.fr",HDTV (720p)
#EXTGRP:
#EXTIMG:https://y.imyr.cm/xy70wD.png
#PLAYLIST:HDTV (720p)
http://0.0.0.0/hbbh/stream.m3u8
""";

    public static readonly Stack<string> SampleTagsExtinfLines
        = new(SampleTagsExtinfContent.Split(["\r\n", "\n", "\r"], StringSplitOptions.None));

    public static readonly Channel SampleTagsExtinfChannel = new()
    {
        Title = "HDTV (720p)",
        Duration = "-1",
        GroupTitle = "",
        TvgID = "HDTV.fr",
        Logo = "https://y.imyr.cm/xy70wD.png",
        TvgName = null,
        MediaUrl = "http://0.0.0.0/hbbh/stream.m3u8",
    };

    // ****************************************** //

    public const string SampleM3UContent = """
#EXTM3U
#EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="",HDTV (720p) [Not 24/7]
http://0.0.0.0/hbbh/stream.m3u8
#EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://y.imyr.cm/xy70wD.png" group-title="Undefined",HDTV (720p)
http://0.0.0.0/hbbh/stream.m3u8
""";

    public static readonly Stack<string> SampleM3ULinesStack
        = new(SampleM3UContent.Split(["\r\n", "\n", "\r"], StringSplitOptions.None));

    public static readonly List<string> SampleM3ULinesList
        = new(SampleM3UContent.Split(["\r\n", "\n", "\r"], StringSplitOptions.None));
}