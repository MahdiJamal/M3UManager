using M3UManager.Helpers;
using M3UManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace M3UManager;

public static class M3UManager
{
    private const string M3UFileStartLineStartWith = "#EXTM3U";

    public static async Task<M3U> ParseFromUrlAsync(string url, NetworkCredential networkCredential)
    {
        using HttpClientHandler handler = new() { Credentials = networkCredential };
        using HttpClient tempHttpClient = new(handler);
        return await ParseFromUrlAsync(url, tempHttpClient);
    }
    public static async Task<M3U> ParseFromUrlAsync(string url)
    {
        using HttpClient tempHttpClient = new();
        return await ParseFromUrlAsync(url, tempHttpClient);
    }
    public static async Task<M3U> ParseFromUrlAsync(string url, HttpClient client)
    {
        HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
        string httpResponseMessageString = await httpResponseMessage.Content.ReadAsStringAsync();
        return ParseFromString(httpResponseMessageString);
    }
    public static M3U ParseFromFile(string m3uFilePath)
        => ParseFromLines(File.ReadAllLines(m3uFilePath));
    public static M3U ParseFromString(string m3uFileContent)
        => ParseFromLines(m3uFileContent.Split("\r\n", "\n", "\r"));
    public static M3U ParseFromLines(IList<string> m3uFileLines)
    {
        if (m3uFileLines == null)
            throw new ArgumentNullException($"The '{nameof(m3uFileLines)}' variable value is null.");
        if (m3uFileLines.Count == 0)
            throw new ArgumentOutOfRangeException($"The '{nameof(m3uFileLines)}' array is empty.");
        if (m3uFileLines[0].StartsWith(M3UFileStartLineStartWith) == false)
            throw new InvalidOperationException($"The m3u file must start with '{M3UFileStartLineStartWith}'.");

        M3U outputM3U = new();

        // Processing in all lines except the first line
        Stack<string> tempStackString = new();
        for (int i = 1; i < m3uFileLines.Count; i++)
        {
            string m3uFileLine = m3uFileLines[i];

            bool successfulyDetectedM3ULineTag = TryDetectM3ULineTag(m3uFileLine, out var detectedM3ULineTag);

            if (successfulyDetectedM3ULineTag)
            {
                var setPropertyByTagNameResult = TrySetM3UPropertyByTagName(ref outputM3U, detectedM3ULineTag);

                if (setPropertyByTagNameResult == SetM3UPropertyResult.Else)
                    tempStackString.Push(m3uFileLine);
            }
            else if (Uri.IsWellFormedUriString(m3uFileLines[i], UriKind.RelativeOrAbsolute))
            {
                tempStackString.Push(m3uFileLine);

                // Expecting tempStackString value is:
                // #EXTINF:-1 tvg-id="AsVr" tvg-logo="https://imur.cm/gqFD.png" group-title="Unined",Asi
                // #EXTGRP:Undefined
                // https://le.as.v/aslv/iex.m3u8

                try
                {
                    Channel detectedChannelFromExtinfItem = DetectChannelFromExtinfItem(tempStackString);
                    outputM3U.Channels.Add(detectedChannelFromExtinfItem);
                }
                catch { }

                tempStackString.Clear();
            }
        }

        return outputM3U;
    }




    private static SetM3UPropertyResult TrySetM3UPropertyByTagName(ref M3U m3u, KeyValuePair<string, string> tagKeyAndName)
    {
        try
        {
            if (m3u == null)
                throw new ArgumentNullException($"'{nameof(m3u)}' variable value is null.");
            if (tagKeyAndName.Key == null)
                throw new ArgumentNullException($"'{nameof(tagKeyAndName.Key)}' variable value is null.");

            switch (tagKeyAndName.Key)
            {
                case "EXT-X-PLAYLIST-TYPE":
                    m3u.PlayListType = tagKeyAndName.Value;
                    break;
                case "EXT-X-TARGETDURATION":
                    m3u.TargetDuration = int.Parse(tagKeyAndName.Value);
                    break;
                case "EXT-X-VERSION":
                    m3u.Version = int.Parse(tagKeyAndName.Value);
                    break;
                case "EXT-X-MEDIA-SEQUENCE":
                    m3u.MediaSequence = int.Parse(tagKeyAndName.Value);
                    break;
                case "EXT-X-ENDLIST":
                    m3u.HasEndList = true;
                    break;
                default:
                    return SetM3UPropertyResult.Else;
            }
        }
        catch
        {
            return SetM3UPropertyResult.Catch;
        }

        return SetM3UPropertyResult.Successful;
    }
    private static SetM3UChannelPropertyResult TrySetM3UChannelPropertyByTagName(ref Channel m3uChannel, KeyValuePair<string, string> tagKeyAndName)
    {
        try
        {
            if (m3uChannel == null)
                throw new ArgumentNullException($"'{nameof(m3uChannel)}' variable value is null.");
            if (tagKeyAndName.Key == null)
                throw new ArgumentNullException($"'{nameof(tagKeyAndName.Key)}' variable value is null.");

            switch (tagKeyAndName.Key)
            {
                case "EXTGRP":
                    m3uChannel.GroupTitle = tagKeyAndName.Value;
                    break;
                case "PLAYLIST":
                    m3uChannel.Title = tagKeyAndName.Value;
                    break;
                case "EXTIMG":
                    m3uChannel.Logo = tagKeyAndName.Value;
                    break;
                case "EXTINF":
                    TrySetM3UChannelPropertyByExtinfAttributes(ref m3uChannel, tagKeyAndName.Value);
                    break;
                default:
                    return SetM3UChannelPropertyResult.Else;
            }
        }
        catch
        {
            return SetM3UChannelPropertyResult.Catch;
        }

        return SetM3UChannelPropertyResult.Successful;
    }
    private static bool TrySetM3UChannelPropertyByExtinfAttributes(ref Channel m3uChannel, string tagValue)
    {
        try
        {
            if (m3uChannel == null)
                throw new ArgumentNullException($"'{nameof(m3uChannel)}' variable value is null.");
            if (tagValue == null)
                throw new ArgumentNullException($"'{nameof(tagValue)}' variable value is null.");

            var extinfAttributes = ExtractExtinfAttributes(tagValue);
            m3uChannel.TvgID = extinfAttributes.GetValueOrDefault("tvg-id", m3uChannel.TvgID);
            m3uChannel.TvgName = extinfAttributes.GetValueOrDefault("tvg-name", m3uChannel.TvgName);
            m3uChannel.Logo = extinfAttributes.GetValueOrDefault("tvg-logo", m3uChannel.Logo);
            m3uChannel.GroupTitle = extinfAttributes.GetValueOrDefault("group-title", m3uChannel.GroupTitle);

            if (tagValue.Contains(','))
                m3uChannel.Title = tagValue.Remove(0, (tagValue.Split(',')[0] + ',').Length);

            m3uChannel.Duration = tagValue.Split(' ')[0];

            return true;
        }
        catch
        {
            return false;
        }
    }
    private static bool TryDetectM3ULineTag(string m3uTagLine, out KeyValuePair<string, string> outputTag)
    {
        try
        {
            if (m3uTagLine == null)
                throw new ArgumentNullException($"'{nameof(m3uTagLine)}' variable value is null.");

            const string regexPatternToGetKey = @"^#(?<key>[a-zA-Z-]+)?";
            const string regexPatternToGetKeyAndValue = $"{regexPatternToGetKey}:(?<value>.+)?";

            bool m3uTagLineContainColon = m3uTagLine.Contains(':');

            string regexPattern = m3uTagLineContainColon ? regexPatternToGetKeyAndValue : regexPatternToGetKey;
            Match regexMatch = Regex.Match(m3uTagLine, regexPattern);

            outputTag = new(regexMatch.Groups["key"].Value, m3uTagLineContainColon ? regexMatch.Groups["value"].Value : null);

            return regexMatch.Success;
        }
        catch { }

        return false;
    }








    /// <summary>
    /// Extinf tag attributes segmentation.
    /// </summary>
    /// <param name="extinfTagAttributesWithoutTagName">
    /// <example>
    /// -1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="Undefined",HDTV(720p) [Not 24/7]
    /// </example>
    /// </param>
    /// <returns>
    /// <example>
    /// <code>
    /// Dictionary&lt;string,string&gt; returnedValue = new Dictionary&lt;string,string&gt;();
    /// returnedValue.Add("group-title", "Undefined");
    /// </code>
    /// </example>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static Dictionary<string, string> ExtractExtinfAttributes(string extinfTagAttributesWithoutTagName)
    {
        if (extinfTagAttributesWithoutTagName == null)
            throw new ArgumentNullException($"'{nameof(extinfTagAttributesWithoutTagName)}' variable value is null.");

        const string regexPattern = @"(?<=\s|\n|^)([a-z\-]+)=\""(.+?)\""";

        return Regex.Matches(extinfTagAttributesWithoutTagName, regexPattern, RegexOptions.IgnoreCase)
                                .Cast<Match>()
                                .ToDictionary(match => match.Groups[1].Value, match => match.Groups[2].Value);
    }

    /// <summary>
    /// This method for parse <see cref="string"/> to <see cref="Channel"/>.
    /// </summary>
    /// <param name="extinfItemLines">
    /// <example>
    /// <code>
    /// Stack&lt;string&gt; inputStack = new Stack&lt;string&gt;();
    /// inputStack.Push("#EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="Undefined",HDTV (720p) [Not 24/7]");
    /// inputStack.Push("http://0.0.0.0/hbbh/stream.m3u8");
    /// </code>
    /// </example>
    /// 
    /// Or
    /// 
    /// <example>
    /// <code>
    /// Stack&lt;string&gt; inputStack = new Stack&lt;string&gt;();
    /// inputStack.Push("#EXTINF:-1 tvg-id="HDTV.fr" tvg-logo="https://o.imur.om/xyW0wD.png" group-title="Undefined",HDTV (720p) [Not 24/7]");
    /// inputStack.Push("#EXTIMG:https://y.imyr.cm/xy70wD.png");
    /// inputStack.Push("#PLAYLIST:HDTV (720p)");
    /// inputStack.Push("#EXTGRP:Undefined");
    /// inputStack.Push("http://0.0.0.0/hbbh/stream.m3u8");
    /// </code>
    /// </example>
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static Channel DetectChannelFromExtinfItem(Stack<string> extinfItemLines)
    {
        if (extinfItemLines == null)
            throw new ArgumentNullException($"'{nameof(extinfItemLines)}' variable value is null.");
        if (extinfItemLines.Count < 2)
            throw new ArgumentOutOfRangeException($"'{nameof(extinfItemLines)}' variable items count must be equal or than of '2'.");

        Channel outputChannel = new()
        {
            MediaUrl = extinfItemLines.Pop()
        };

        while (extinfItemLines.Count > 0)
        {
            string extinfItemLine = extinfItemLines.Pop();

            bool successfulyDetectedM3ULineTag = TryDetectM3ULineTag(extinfItemLine, out var detectedM3ULineTag);

            if (successfulyDetectedM3ULineTag)
                TrySetM3UChannelPropertyByTagName(ref outputChannel, detectedM3ULineTag);
        }

        // Return output
        return outputChannel;
    }
}
