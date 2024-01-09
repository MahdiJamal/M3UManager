using M3UManager.Models;
using M3UManager.Utilities;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace M3UManager;

public static class M3UManager
{
    private const string M3UFileStartLine = "#EXTM3U";

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
    public static async Task<M3U> ParseFromFileAsync(string m3uFilePath)
        => ParseFromLines(await File.ReadAllLinesAsync(m3uFilePath));
    public static M3U ParseFromString(string m3uFileContent)
        => ParseFromLines(m3uFileContent.Split("\r\n", "\n", "\r"));
    public static M3U ParseFromLines(string[] m3uFileLines)
    {
        if (m3uFileLines == null)
            throw new ArgumentNullException($"The '{nameof(m3uFileLines)}' variable value is null.");
        if (m3uFileLines.Length == 0)
            throw new ArgumentOutOfRangeException($"The '{nameof(m3uFileLines)}' array is empty.");
        if (m3uFileLines[0].StartsWith(M3UFileStartLine) == false)
            throw new InvalidOperationException($"The m3u file must start with '{M3UFileStartLine}'.");

        M3U m3u = new();
        Media tempMedia = new();

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
                        tempMedia.ExtinfTag = new ExtinfTag(ExtinfTagAttributes.Parse(tagValueWithoutKey));
                        break;
                    case "#EXTGRP":
                        tempMedia.ExtinfTag.TagAttributes.GroupTitle = tagValueWithoutKey;
                        break;

                    case "#EXT-X-ENDLIST":
                        m3u.HasEndList = true;
                        break;
                }
            }
            else if (m3uFileLines[i].StartsWith("http"))
            {
                tempMedia.MediaUri = new Uri(m3uFileLines[i]);
                m3u.Medias.Add(tempMedia.Clone<Media>());
                tempMedia.Reset();
            }
        }

        return m3u;
    }
}
