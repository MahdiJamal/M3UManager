using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

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


    public void SaveM3UFile(string filePathToSave, M3UType groupTitle = M3UType.TagsType)
        => File.WriteAllLines(filePathToSave, CreateM3ULines(groupTitle));
    public string CreateM3UText(M3UType groupTitle = M3UType.TagsType)
    {
        StringBuilder stringBuilder = new();

        foreach (string line in CreateM3ULines(groupTitle))
            stringBuilder.AppendLine(line);

        return stringBuilder.ToString();
    }
    public IEnumerable<string> CreateM3ULines(M3UType groupTitle = M3UType.TagsType)
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
            yield return "#EXT-X-ENDLIST";
    }
}
