using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCAMDecryptor {
    public class OtSpawn {
        public Location Location { get; private set; }
        public int Radius { get; private set; }

        private readonly OtCreature[,] creatures;
        private readonly int size;
        private int count;

        public OtSpawn(Location location, int radius) {
            this.Location = location;
            this.Radius = radius;
            this.size = (radius * 2) + 1;
            this.count = 0;
            creatures = new OtCreature[size, size];
        }

        public bool AddCreature(OtCreature creature) {
            if (count >= 9) 
                return false;

            var newCreature = new OtCreature() { Location = RelativeSpiralCoordinates(count, creature.Location.Z), Name = creature.Name, Type = creature.Type };
            count++;

            if (creatures[newCreature.Location.X + Radius, newCreature.Location.Y + Radius] == null) {
                creatures[newCreature.Location.X + Radius, newCreature.Location.Y + Radius] = newCreature;
                return true;
            }

            return false;
        }

        public IEnumerable<OtCreature> GetCreatures() {
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    if (creatures[x, y] != null)
                        yield return creatures[x, y];
                }
            }
        }

        public static Location RelativeSpiralCoordinates(int counter, int z)
        {
            if (counter == 0) return new Location(0, 0, z);
            if (counter == 1) return new Location(1, 0, z);
            if (counter == 2) return new Location(1, 1, z);
            if (counter == 3) return new Location(0, 1, z);
            if (counter == 4) return new Location(-1, 1, z);
            if (counter == 5) return new Location(-1, 0, z);
            if (counter == 6) return new Location(-1, -1, z);
            if (counter == 7) return new Location(0, -1, z);
            if (counter == 8) return new Location(1, -1, z);
            return null;
        }

    }
}
