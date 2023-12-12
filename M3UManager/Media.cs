using M3UManager.Models;
using System;
using System.Collections.Generic;

namespace M3UManager
{
    public class Media : ICloneable
    {
        public Uri StreamUri { get; set; }
        public ExtinfTag ExtinfTag { get; set; }

        public object Clone()
            => this.MemberwiseClone();
        public T Clone<T>()
            => (T)this.Clone();

        public void Reset()
        {
            StreamUri = null;
            ExtinfTag = null;
        }
    }
}
