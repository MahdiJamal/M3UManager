using System;
using System.Collections.Generic;
using System.IO;

namespace M3UManager.Models;

public partial class M3U
{
    public string PlayListType { get; set; }
    public bool HasEndList { get; set; }
    public int? TargetDuration { get; set; }
    public int? Version { get; set; }
    public int? MediaSequence { get; set; }
    public List<Media> Medias { get; set; } = [];

    public void SaveToFile(string filePathToSave, M3UGroupTitle groupTitle = M3UGroupTitle.InlineGroupTitle)
        => File.WriteAllLines(filePathToSave, GetM3UAsString(groupTitle));
    public IEnumerable<string> GetM3UAsString(M3UGroupTitle groupTitle = M3UGroupTitle.InlineGroupTitle)
    {
        yield return "#EXTM3U";

        if (PlayListType != null)
            yield return $"#EXT-X-PLAYLIST-TYPE:{PlayListType}";

        if (TargetDuration != null)
            yield return $"#EXT-X-TARGETDURATION:{TargetDuration}";

        if (Version != null)
            yield return $"#EXT-X-VERSION:{Version}";

        if (MediaSequence != null)
            yield return $"#EXT-X-MEDIA-SEQUENCE:{MediaSequence}";

        switch (groupTitle)
        {
            case M3UGroupTitle.InlineGroupTitle:
                foreach (Media media in Medias)
                {
                    yield return $"#EXTINF:{media.ExtinfTag.TagAttributes}";
                    yield return media.StreamUri.AbsoluteUri;
                }
                break;
            case M3UGroupTitle.OutlineGroupTitle:
                throw new NotImplementedException();
        }

        if (HasEndList)
            yield return $"#EXT-X-ENDLIST";
    }
}
