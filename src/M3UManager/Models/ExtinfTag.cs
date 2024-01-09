using CommunityToolkit.Mvvm.ComponentModel;

namespace M3UManager.Models;

public partial class ExtinfTag(ExtinfTagAttributes extinfTagAttributes) : ObservableObject
{
    [ObservableProperty]
    private ExtinfTagAttributes _tagAttributes = extinfTagAttributes;
}
