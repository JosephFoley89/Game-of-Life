using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_of_Life {
    internal class Coords {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x = 0, int y = 0) {
            X = x;
            Y = y;
        }

        public override string ToString() { return $"{X},{Y}"; }
    }
}
