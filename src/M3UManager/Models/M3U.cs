using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace M3UManager.Models;

public partial class M3U : ObservableObject
{
    [ObservableProperty]
    private string _playListType = null;

    [ObservableProperty]
    private bool _hasEndList = false;

    [ObservableProperty]
    private int? _targetDuration = null;

    [ObservableProperty]
    private int? _version = null;

    [ObservableProperty]
    private int? _mediaSequence = null;

    [ObservableProperty]
    private ObservableCollection<Media> _medias = [];

    public async Task SaveM3UFileAsync(string filePathToSave, M3UGroupTitle groupTitle = M3UGroupTitle.InlineGroupTitle)
        => await File.WriteAllLinesAsync(filePathToSave, CreateM3ULines(groupTitle));
    public void CreateM3UText(M3UGroupTitle groupTitle = M3UGroupTitle.InlineGroupTitle)
        => string.Join("\r\n", CreateM3ULines(groupTitle));
    public IEnumerable<string> CreateM3ULines(M3UGroupTitle groupTitle = M3UGroupTitle.InlineGroupTitle)
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
                    yield return media.MediaUri.AbsoluteUri;
                }
                break;
            case M3UGroupTitle.OutlineGroupTitle:
                throw new NotImplementedException();
        }

        if (HasEndList)
            yield return $"#EXT-X-ENDLIST";
    }
}
