using System;

namespace M3UManager.Models
{
    public class Media : ICloneable
    {
        public Uri StreamUri { get; set; }
        public ExtinfTag ExtinfTag { get; set; }

        public object Clone()
            => MemberwiseClone();
        public T Clone<T>()
            => (T)Clone();

        public void Reset()
        {
            StreamUri = null;
            ExtinfTag = null;
        }
    }
}
