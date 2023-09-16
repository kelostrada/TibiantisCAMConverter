using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCAMDecryptor
{
    public class MapItem
    {
        public ushort id;
        public byte count;

        public MapItem(ushort id, byte count)
        {
            this.id = id;
            this.count = count;
        }
    }
}
