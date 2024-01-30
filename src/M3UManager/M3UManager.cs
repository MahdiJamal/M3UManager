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
    public static M3U ParseFromLines(string[] m3uFileLines)
    {
        if (m3uFileLines == null)
            throw new ArgumentNullException($"The '{nameof(m3uFileLines)}' variable value is null.");
        if (m3uFileLines.Length == 0)
            throw new ArgumentOutOfRangeException($"The '{nameof(m3uFileLines)}' array is empty.");
        if (m3uFileLines[0].StartsWith(M3UFileStartLineStartWith) == false)
            throw new InvalidOperationException($"The m3u file must start with '{M3UFileStartLineStartWith}'.");

        M3U outputM3U = new();

        // Processing in all lines except the first line
        Stack<string> tempStackString = new();
        for (int i = 1; i < m3uFileLines.Length; i++)
        {
            string m3uFileLine = m3uFileLines[i];

            if (m3uFileLine.StartsWith("#EXT-X"))
            {
                string tagKey = m3uFileLine.Split(':')[0];
                string tagValueWithoutKey = m3uFileLine.Remove(0, (tagKey + ':').Length);
                switch (tagKey)
                {
                    case "#EXT-X-PLAYLIST-TYPE":
                        outputM3U.PlayListType = tagValueWithoutKey;
                        break;
                    case "#EXT-X-TARGETDURATION":
                        outputM3U.TargetDuration = int.Parse(tagValueWithoutKey);
                        break;
                    case "#EXT-X-VERSION":
                        outputM3U.Version = int.Parse(tagValueWithoutKey);
                        break;
                    case "#EXT-X-MEDIA-SEQUENCE":
                        outputM3U.MediaSequence = int.Parse(tagValueWithoutKey);
                        break;
                    case "#EXT-X-ENDLIST":
                        outputM3U.HasEndList = true;
                        break;
                }
            }
            else if (m3uFileLine.StartsWith("#"))
            {
                tempStackString.Push(m3uFileLine);
            }
            else if (Uri.IsWellFormedUriString(m3uFileLines[i], UriKind.RelativeOrAbsolute))
            {
                tempStackString.Push(m3uFileLine);

                // Expecting tempListString value is:
                // #EXTINF:-1 tvg-id="AsVr" tvg-logo="https://imur.cm/gqFD.png" group-title="Unined",Asi
                // #EXTGRP:Undefined
                // https://le.as.v/aslv/iex.m3u8

                outputM3U.Channels.Add(DetectChannelFromExtinfItem(tempStackString));

                tempStackString.Clear();
            }
        }

        return outputM3U;
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
    /// <exception cref="NullReferenceException"></exception>
    private static Dictionary<string, string> ExtractExtinfAttributes(string extinfTagAttributesWithoutTagName)
    {
        if (extinfTagAttributesWithoutTagName == null)
            throw new NullReferenceException($"'{nameof(extinfTagAttributesWithoutTagName)}' variable value is null.");

        const string pattern = @"(?<=\s|\n|^)([a-z\-]+)=\""(.+?)\""";

        return Regex.Matches(extinfTagAttributesWithoutTagName, pattern, RegexOptions.IgnoreCase)
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
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static Channel DetectChannelFromExtinfItem(Stack<string> extinfItemLines)
    {
        if (extinfItemLines == null)
            throw new NullReferenceException($"'{nameof(extinfItemLines)}' variable value is null.");
        if (extinfItemLines.Count < 2)
            throw new ArgumentOutOfRangeException($"'{nameof(extinfItemLines)}' variable items count must be equal or than of '2'.");

        Channel outputChannel = new()
        {
            MediaUrl = extinfItemLines.Pop()
        };

        // Detect tags
        if (extinfItemLines.Count != 1)
            for (int i = 0; i <= extinfItemLines.Count; i++)
            {
                string extinfItemLine = extinfItemLines.Pop();
                string tagKey = extinfItemLine.Split(':')[0];
                string tagValueWithoutKey = extinfItemLine.Remove(0, (tagKey + ':').Length);

                switch (tagKey)
                {
                    case "#EXTGRP": outputChannel.GroupTitle = tagValueWithoutKey; break;
                    case "#PLAYLIST": outputChannel.Title = tagValueWithoutKey; break;
                    case "#EXTIMG": outputChannel.Logo = tagValueWithoutKey; break;
                }
            }

        // Detect attributes
        string extinfTagFirstLine = extinfItemLines.Pop();
        string extinfTagAttributesWithoutTagName = extinfTagFirstLine.Remove(0, /* "#EXTINF:".Length */ 8);
        var extinfAttributes = ExtractExtinfAttributes(extinfTagAttributesWithoutTagName);
        outputChannel.TvgID ??= extinfAttributes.GetValueOrDefault("tvg-id", null);
        outputChannel.TvgName ??= extinfAttributes.GetValueOrDefault("tvg-name", null);
        outputChannel.Logo ??= extinfAttributes.GetValueOrDefault("tvg-logo", null);
        outputChannel.GroupTitle ??= extinfAttributes.GetValueOrDefault("group-title", null);

        if (extinfTagAttributesWithoutTagName.Contains(','))
            outputChannel.Title ??= extinfTagAttributesWithoutTagName.Remove(0, (extinfTagAttributesWithoutTagName.Split(',')[0] + ',').Length);

        outputChannel.Duration ??= extinfTagAttributesWithoutTagName.Split(' ')[0];

        // Return output
        return outputChannel;
    }
}
