namespace M3UManager.Models;

public partial class ExtinfTag(ExtinfTagAttributes extinfTagAttributes) : NotifyPropertyChanged
{
    private ExtinfTagAttributes _tagAttributes = extinfTagAttributes;
    public ExtinfTagAttributes TagAttributes { get => _tagAttributes; set => SetProperty(ref _tagAttributes, value); }
}
