using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryScrobbler.Lib
{
    public class BiDirectionary<T, K>
    {
        private readonly Dictionary<T, K> _first = new Dictionary<T, K>();
        private readonly Dictionary<K, T> _second = new Dictionary<K, T>();

        public BiDirectionary()
        {
        }

        public void Add(T first, K second)
        {
            _first.Add(first, second);
            _second.Add(second, first);
        }

        public T GetFirst(K value)
        {
            return _second[value];
        }

        public K GetSecond(T value)
        {
            return _first[value];
        }
    }
}
