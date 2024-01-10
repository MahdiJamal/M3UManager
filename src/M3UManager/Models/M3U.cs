using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace M3UManager.Models;

public partial class M3U : NotifyPropertyChanged
{
    private string _playListType = null;
    public string PlayListType { get => _playListType; set => SetProperty(ref _playListType, value); }

    private bool _hasEndList = false;
    public bool HasEndList { get => _hasEndList; set => SetProperty(ref _hasEndList, value); }

    private int? _targetDuration = null;
    public int? TargetDuration { get => _targetDuration; set => SetProperty(ref _targetDuration, value); }

    private int? _version = null;
    public int? Version { get => _version; set => SetProperty(ref _version, value); }

    private int? _mediaSequence = null;
    public int? MediaSequence { get => _mediaSequence; set => SetProperty(ref _mediaSequence, value); }

    private ObservableCollection<Channel> _channels = [];
    public ObservableCollection<Channel> Channels { get => _channels; set => SetProperty(ref _channels, value); }


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

        foreach (Channel channel in Channels)
            yield return channel.ChannelToString(groupTitle);

        if (HasEndList)
            yield return $"#EXT-X-ENDLIST";
    }
}
