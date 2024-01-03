using M3UManager.Models;
using System;
using System.IO;

namespace M3UManager
{
    public static class M3UManager
    {
        private const string M3UFileStartLine = "#EXTM3U";

        public static M3U Parse(string m3uFilePath)
            => Parse(File.ReadAllLines(m3uFilePath));
        public static M3U Parse(string[] m3uFileContent)
        {
            if (m3uFileContent == null)
                throw new ArgumentNullException($"The '{nameof(m3uFileContent)}' variable value is null.");
            if (m3uFileContent.Length == 0)
                throw new ArgumentOutOfRangeException($"The '{nameof(m3uFileContent)}' array is empty.");
            if (m3uFileContent[0].StartsWith(M3UFileStartLine) == false)
                throw new InvalidOperationException($"The m3u file must start with '{M3UFileStartLine}'.");

            M3U m3u = new M3U();
            Media tempMedia = new Media();

            // Processing in all lines except the first line
            for (int i = 1; i < m3uFileContent.Length; i++)
            {
                if (!m3uFileContent[i].StartsWith("##") && m3uFileContent[i].StartsWith("#"))
                {
                    string tagKey = m3uFileContent[i].Split(':')[0];
                    string tagValueWithoutKey = m3uFileContent[i].Replace(tagKey + ':', "");

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
                else if (m3uFileContent[i].StartsWith("http"))
                {
                    tempMedia.StreamUri = new Uri(m3uFileContent[i]);
                    m3u.Medias.Add(tempMedia.Clone<Media>());
                    tempMedia.Reset();
                }
            }

            return m3u;
        }
    }
}
