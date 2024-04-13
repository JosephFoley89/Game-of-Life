using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Game_of_Life {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
        private static int BoardHeight = 1000;
        private static int BoardWidth = 1000;
        private static int CellSize = 20;
        private static int LifeChance = 69;
        private static int TickRate = 250;
        private int generation = 0;
        private double percentageAlive = 0;
        private System.Timers.Timer timer;
        private bool mouseDown = false;

        public MainWindow() {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            this.MouseDown += MouseState;
            this.MouseUp += MouseState;
            cells = CreateBoard();
            SetNeighbors(cells);
        }

        private void MouseState(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                mouseDown = true;
            } else {
                mouseDown = false;
            }
        }

        private void SetTimer() {
            timer = new System.Timers.Timer();
            timer.Interval = TickRate;
            timer.Elapsed += Tick;
            timer.Start();
        }

        private void StopTimer() {
            if (timer != null) {
                generation = 0;
                timer.Stop();
            }
        }

        private void SetNeighbors(Dictionary<string, Cell> cells) {
            foreach (Cell cell in cells.Values) {
                List<string> coords = new List<string>();
                coords.Add($"{cell.Coordinates.X},{cell.Coordinates.Y + 1}");
                coords.Add($"{cell.Coordinates.X + 1},{cell.Coordinates.Y + 1}");
                coords.Add($"{cell.Coordinates.X + 1},{cell.Coordinates.Y}");
                coords.Add($"{cell.Coordinates.X + 1},{cell.Coordinates.Y - 1}");
                coords.Add($"{cell.Coordinates.X},{cell.Coordinates.Y - 1}");
                coords.Add($"{cell.Coordinates.X - 1},{cell.Coordinates.Y + 1}");
                coords.Add($"{cell.Coordinates.X - 1},{cell.Coordinates.Y}");
                coords.Add($"{cell.Coordinates.X - 1},{cell.Coordinates.Y - 1}");

                foreach (string key in coords) {
                    if (cells.ContainsKey(key)) {
                        cell.Neighbors.Add(cells[key]);
                    }
                }
            }
        }

        private Dictionary<string, Cell> CreateBoard() {
            Dictionary<string, Cell> board = new Dictionary<string, Cell>();
            Canvas canvas = new Canvas();
            canvas.Background = Brushes.White;
            canvas.Width = BoardWidth;
            canvas.Height = BoardHeight;

            for (int x = 0; x < BoardWidth; x += CellSize) {
                for (int y = 0; y < BoardHeight; y += CellSize) {
                    Rectangle cellShape = new Rectangle();
                    cellShape.Height = CellSize;
                    cellShape.Width = CellSize;
                    cellShape.MouseDown += UpdateCell;
                    cellShape.MouseEnter += UpdateCellDrag;
                    cellShape.Name = $"cell_{x/CellSize}_{y/CellSize}";
                    canvas.Children.Add(cellShape);
                    Canvas.SetLeft(cellShape, x);
                    Canvas.SetTop(cellShape, y);

                    Coords coords = new Coords(x / CellSize, y / CellSize);
                    Cell cell = new Cell(coords, cellShape, false);
                    board.Add(coords.ToString(), cell);
                }
            }

            GameBoard.Children.Add(canvas);
            return board;
        }

        private void UpdateCell(object sender, EventArgs e) {
            if (timer == null) {
                Rectangle rect = (Rectangle)sender;
                string key = rect.Name.Replace("cell_", string.Empty).Replace("_", ",");
                cells[key].IsAlive = !cells[key].IsAlive;
                cells[key].Update();
            } else if (!timer.Enabled) {
                Rectangle rect = (Rectangle)sender;
                string key = rect.Name.Replace("cell_", string.Empty).Replace("_", ",");
                cells[key].IsAlive = !cells[key].IsAlive;
                cells[key].Update();
            }
        }

        private void UpdateCellDrag(object sender, EventArgs e) {
            if (mouseDown) {
                if (timer == null) {
                    Rectangle rect = (Rectangle)sender;
                    string key = rect.Name.Replace("cell_", string.Empty).Replace("_", ",");
                    cells[key].IsAlive = !cells[key].IsAlive;
                    cells[key].Update();
                } else if (!timer.Enabled) {
                    Rectangle rect = (Rectangle)sender;
                    string key = rect.Name.Replace("cell_", string.Empty).Replace("_", ",");
                    cells[key].IsAlive = !cells[key].IsAlive;
                    cells[key].Update();
                }
            }
        }

        private void Tick(object sender, EventArgs e) {
            generation++;
            percentageAlive = (double)cells.Values.Where(x => x.IsAlive).Count() / (double)cells.Values.Count();
            this.Dispatcher.Invoke(() => EvaluateRelationships(cells, generation));
            this.Dispatcher.Invoke(() => Generation.Content = $"Generation: {generation}");
            this.Dispatcher.Invoke(() => PercentageAlive.Content = $"Alive: {(percentageAlive).ToString("0.00%")}");
        }

        private static void EvaluateRelationships(Dictionary<string, Cell> cells, int generation) {
            List<bool> fates = new List<bool>();

            foreach (Cell cell in cells.Values) {
                fates.Add(DetermineFate(cell, cell.Neighbors.Where(x => x.IsAlive).Count()));
            }

            for (int i = 0; i < fates.Count(); i++) {
                cells.ElementAt(i).Value.IsAlive = fates[i];
                cells.ElementAt(i).Value.Update();
            }
        }


        private static bool DetermineFate(Cell cell, int livingNeighbors) {
            bool result = false;
            if (cell.IsAlive) {
                if (livingNeighbors < 2) {
                    result = false;
                } else if (livingNeighbors == 2 || livingNeighbors == 3) {
                    result = true;
                } else if (livingNeighbors > 3) {
                    result = false;
                }
            }
            else if (cell.IsAlive == false && livingNeighbors == 3) {
                result = true;
            }

            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SetTimer();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            StopTimer();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            StopTimer();
            Random random = new Random();

            foreach (Cell cell in cells.Values) {
                cell.IsAlive = random.Next(0, 100) > LifeChance;
                cell.Update();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            StopTimer();
            foreach (Cell cell in cells.Values) {
                cell.IsAlive = false;
                cell.Update();
            }
        }
    }
}