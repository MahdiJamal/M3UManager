using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace M3UManager.Models;

public partial class Media : ObservableObject, ICloneable
{
    [ObservableProperty]
    private Uri _mediaUri;

    [ObservableProperty]
    private ExtinfTag _extinfTag;

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
