namespace M3UManager.Models
{
    public class ExtinfTag
    {
        public ExtinfTagAttributes TagAttributes { get; set; }

        public ExtinfTag(ExtinfTagAttributes extinfTagAttributes)
        {
            this.TagAttributes = extinfTagAttributes;
        }
    }
}
