using System.Windows.Media;
using System.Windows.Shapes;

namespace Game_of_Life {
    internal class Cell {
        public Coords Coordinates { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsAlive { get; set; }
        public List<Cell> Neighbors { get; set; }

        public Cell(Coords coordinates, Rectangle bounds, bool isAlive, List<Cell> neighbors = null) {
            Coordinates = coordinates;
            Bounds = bounds;
            IsAlive = isAlive;
            Neighbors = neighbors == null ? new List<Cell>() : neighbors;
            Bounds.Fill = isAlive ? Brushes.Black : Brushes.WhiteSmoke;
        }

        public void Update() {
            Bounds.Fill = IsAlive ? Brushes.Black : Brushes.WhiteSmoke;
        }

        public override string ToString() {
            return $"{Coordinates}";
        }

    }
}
