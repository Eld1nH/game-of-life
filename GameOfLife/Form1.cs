using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        public enum DrawMode
        {
            Disabled = 0,
            Live = 1,
            Die = 2
        }

        private int _cellWidth = 10;
        private int _cellHeight = 10;

        private int _width;
        private int _height;

        private Cell[,] _cells;

        private DrawMode _drawMode = DrawMode.Disabled;

        public Form1()
        {
            InitializeComponent();

            _width = GridPanel.Width / _cellWidth;
            _height = GridPanel.Height / _cellHeight;

            _cells = new Cell[_width, _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _cells[x, y] = new Cell();
                }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            Cell[,] cellsCopy = new Cell[_width, _height];
            Array.Copy(_cells, 0, cellsCopy, 0, _cells.Length);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int liveNeighbors = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }

                            if (x - i < 0 || x - i > _width - 1 || y - j < 0 || y - j > _height - 1)
                            {
                                continue;
                            }

                            if (_cells[x - i, y - j].IsAlive)
                            {
                                liveNeighbors++;
                            }
                        }
                    }

                    if (cellsCopy[x, y].IsAlive && (liveNeighbors <= 1 || liveNeighbors >= 4))
                    {
                        cellsCopy[x, y].IsAlive = false;
                    }
                    else if (!cellsCopy[x, y].IsAlive && liveNeighbors == 3)
                    {
                        cellsCopy[x, y].IsAlive = true;
                    }
                }
            }

            Array.Copy(cellsCopy, 0, _cells, 0, _cells.Length);
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(GridPanel.Width, GridPanel.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.DarkGray);

                using (var pen = new Pen(Color.LightGray))
                {
                    for (int i = _cellWidth; i < GridPanel.Width; i += _cellWidth)
                    {
                        g.DrawLine(pen, i, 0, i, GridPanel.Height);
                        g.DrawLine(pen, 0, i, GridPanel.Width, i);
                    }
                }

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var r = new Rectangle(x * _cellWidth + (x == 0 ? 0 : 1), y * _cellHeight + (y == 0 ? 0 : 1), _cellWidth - (x == 0 ? 0 : 1), _cellHeight - (y == 0 ? 0 : 1));
                        if (_cells[x, y].IsAlive)
                        {
                            g.FillRectangle(Brushes.White, r);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.DarkGray, r);
                        }
                    }
                }
            }

            var image = GridPanel.Image;
            GridPanel.Image = bitmap;
            if (image != null)
            {
                image.Dispose();
            }
        }

        private void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                UpdateTimer.Enabled = !UpdateTimer.Enabled;
            }
        }

        private void GridPanel_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X / _cellWidth;
            int y = e.Y / _cellHeight;

            _cells[x, y].ToggleStatus();

            _drawMode = _cells[x, y].IsAlive ? DrawMode.Live : DrawMode.Die;
        }

        private void GridPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _drawMode = DrawMode.Disabled;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _cells[x, y].HasBeenToggled = false;
                }
            }
        }

        private void GridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / _cellWidth;
            int y = e.Y / _cellHeight;

            if ((x < 0 || x >= _width) || (y < 0 || y >= _height))
            {
                return;
            }

            if (_cells[x, y].HasBeenToggled)
            {
                return;
            }

            switch (_drawMode)
            {
                case DrawMode.Live:
                    if (!_cells[x, y].IsAlive)
                    {
                        _cells[x, y].ToggleStatus();
                    }
                    break;

                case DrawMode.Die:
                    if (_cells[x, y].IsAlive)
                    {
                        _cells[x, y].ToggleStatus();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
