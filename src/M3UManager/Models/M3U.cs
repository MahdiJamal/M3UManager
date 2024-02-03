using System.Collections.ObjectModel;

namespace M3UManager.Models;

public class M3U : ModelBaseClass
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
}
