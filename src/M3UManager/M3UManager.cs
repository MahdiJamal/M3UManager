using M3UManager.Models;
using M3UManager.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
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

        M3U m3u = new();
        Channel tempChannel = new();

        // Processing in all lines except the first line
        for (int i = 1; i < m3uFileLines.Length; i++)
        {
            if (!m3uFileLines[i].StartsWith("##") && m3uFileLines[i].StartsWith("#"))
            {
                string tagKey = m3uFileLines[i].Split(':')[0];
                string tagValueWithoutKey = m3uFileLines[i].Replace(tagKey + ':', "");

                switch (tagKey)
                {
                    case "#EXT-X-PLAYLIST-TYPE":
                        m3u.PlayListType = tagValueWithoutKey;
                        break;
                    case "#EXT-X-TARGETDURATION":
                        m3u.TargetDuration = int.Parse(tagValueWithoutKey);
                        break;
                    case "#EXT-X-VERSION":
                        m3u.Version = int.Parse(tagValueWithoutKey);
                        break;
                    case "#EXT-X-MEDIA-SEQUENCE":
                        m3u.MediaSequence = int.Parse(tagValueWithoutKey);
                        break;

                    case "#EXTINF":
                        tempChannel.DetectAndSetExtinfProperties(tagValueWithoutKey);
                        break;
                    case "#EXTGRP":
                        tempChannel.GroupTitle = tagValueWithoutKey;
                        break;
                    case "#PLAYLIST":
                        tempChannel.Title = tagValueWithoutKey;
                        break;
                    case "#EXTIMG":
                        tempChannel.TvgLogo = tagValueWithoutKey;
                        break;

                    case "#EXT-X-ENDLIST": m3u.HasEndList = true; break;
                }
            }
            else if (m3uFileLines[i].StartsWith("http"))
            {
                tempChannel.MediaUrl = m3uFileLines[i];
                m3u.Channels.Add(tempChannel.Clone<Channel>());
                tempChannel.Reset();
            }
        }

        return m3u;
    }
}
