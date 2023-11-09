using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCAMDecryptor {
    public class OtCreature {
        public uint Id { get; set; }
        public string Name { get; set; }
        public CreatureType Type { get; set; }
        public Location Location { get; set; }

        public override bool Equals(object obj) {
            var other = obj as OtCreature;
            return other != null && other.Name == Name && other.Location == Location;
        }

        public override int GetHashCode() {
            return Name.GetHashCode() + Location.GetHashCode();
        }
    }
}
