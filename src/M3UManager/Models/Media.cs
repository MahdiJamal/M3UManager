using System;

namespace M3UManager.Models;

public partial class Media : NotifyPropertyChanged, ICloneable
{
    private Uri _mediaUri;
    public Uri MediaUri { get => _mediaUri; set => SetProperty(ref _mediaUri, value); }

    private ExtinfTag _extinfTag;
    public ExtinfTag ExtinfTag { get => _extinfTag; set => SetProperty(ref _extinfTag, value); }

    public object Clone()
        => MemberwiseClone();
    public T Clone<T>()
        => (T)Clone();

    public void Reset()
    {
        MediaUri = null;
        ExtinfTag = null;
    }
}
