using System.Collections.Generic;
using System.IO;

namespace M3UManager.Models
{
    public class M3U
    {
        public string PlayListType { get; set; }
        public bool HasEndList { get; set; }
        public int? TargetDuration { get; set; }
        public int? Version { get; set; }
        public int? MediaSequence { get; set; }
        public List<Media> Medias { get; set; } = new List<Media>();

        public enum GroupTitle
        {
            InlineGroupTitle,
            OutlineGroupTitle
        }
        public void SaveToFile(string filePathToSave, GroupTitle groupTitle = GroupTitle.InlineGroupTitle)
            => File.WriteAllLines(filePathToSave, GetM3UAsString(groupTitle));
        public string[] GetM3UAsString(GroupTitle groupTitle = GroupTitle.InlineGroupTitle)
        {
            List<string> list = new List<string>()
            {
                "#EXTM3U"
            };

            if (PlayListType != null)
                list.Add($"#EXT-X-PLAYLIST-TYPE:{PlayListType}");

            if (TargetDuration != null)
                list.Add($"#EXT-X-TARGETDURATION:{TargetDuration}");

            if (Version != null)
                list.Add($"#EXT-X-VERSION:{Version}");

            if (MediaSequence != null)
                list.Add($"#EXT-X-MEDIA-SEQUENCE:{MediaSequence}");

            switch (groupTitle)
            {
                case GroupTitle.InlineGroupTitle:
                    foreach (Media media in Medias)
                    {
                        list.Add($"#EXTINF:{media.ExtinfTag.TagAttributes}");
                        list.Add(media.StreamUri.AbsoluteUri);
                    }
                    break;
                case GroupTitle.OutlineGroupTitle:
                    foreach (Media media in Medias)
                    {

                    }
                    break;
            }

            if (HasEndList)
                list.Add($"#EXT-X-ENDLIST");

            return list.ToArray();
        }
    }
}
