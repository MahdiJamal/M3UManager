using System.Collections.Generic;

namespace M3UManager
{
    public class M3U
    {
        public string PlayListType { get; set; }
        public bool HasEndList { get; set; }
        public int? TargetDuration { get; set; }
        public int? Version { get; set; }
        public int? MediaSequence { get; set; }
        public List<Media> Medias { get; set; } = new List<Media>();
    }
}
