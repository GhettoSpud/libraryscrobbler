using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryScrobbler.Lib
{
    public class UniversalTag
    {
        public string Name { get; internal set; }
        public string Id3Name { get; internal set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    
}
