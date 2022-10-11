using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _width;
        private int _height;
        private int _speed;
        
        private DispatcherTimer _timer;
        
        private int _days = 0;
        private int _alive = 0;
        private int _dead = 0;
        private int _maxNum = 0;
        private bool _isRunning = false;
        private Dictionary<int, Chart.StatisticFrame> _statistics = new Dictionary<int, Chart.StatisticFrame>();
        
        private Cell[] _cells;
        private Cell[,] _cells2D;

        private bool _showChart = true;
        private Chart _chart;
        
        public MainWindow()
        {
            InitializeComponent();
            
            StartBtn.Visibility = Visibility.Hidden;
            RandomBtn.Visibility = Visibility.Hidden;
            StopBtn.Visibility = Visibility.Hidden;
            ResumeBtn.Visibility = Visibility.Hidden;
            LifeCount.Visibility = Visibility.Hidden;
            DeadCount.Visibility = Visibility.Hidden;
            DayCount.Visibility = Visibility.Hidden;
            
            _width = 30;
            _height = 30;
            _speed = 1500;
            
            SizeSlider.Value = _width;
            SpeedSlider.Value = ((double)_speed / 100);
            ChartCheck.IsChecked = _showChart;
            
            _chart = new Chart(Chart);
            _chart.Hide();
            
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CreateBtn.Visibility = Visibility.Hidden;
            Options.Visibility = Visibility.Hidden;
            _cells = new Cell[_width * _height];
            _cells2D = new Cell[_width, _height];
            _timer.Interval = TimeSpan.FromMilliseconds(_speed);
            CreateCanvas(_width, _height);
            RandomBtn.Visibility = Visibility.Visible;
        }
        
        
        private void RandomBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            for (int i = 0; i < _cells.Length; i++)
            {
                int number = random.Next(0, 3);
                if (number == 1)
                {
                    _cells[i].CellSelect(_cells[i], _cells[i].Ellipse);
                }
                else
                {
                    _cells[i].CellUnselect(_cells[i], _cells[i].Ellipse);
                }
            }
        }

        private void StartBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _isRunning = true;
            StartBtn.Visibility = Visibility.Hidden;
            RandomBtn.Visibility = Visibility.Hidden;
            StopBtn.Visibility = Visibility.Visible;
            DeadCount.Visibility = Visibility.Visible;
            DayCount.Visibility = Visibility.Visible;
            
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i].DrawStartSimulation();
            }

            if (_showChart)
            {
                _statistics.Add(_days, new Chart.StatisticFrame(_days, _alive, _dead));
                _chart.Show();
            }
            _timer.Start();
        }
        
        private void StopBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            StopBtn.Visibility = Visibility.Hidden;
            ResumeBtn.Visibility = Visibility.Visible;
        }
        
        private void ResumeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            ResumeBtn.Visibility = Visibility.Hidden;
            StopBtn.Visibility = Visibility.Visible;
        }
        
        private void SizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _width = (int) SizeSlider.Value;
            _height = (int) SizeSlider.Value;
        }

        private void SpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _speed = ((int) SizeSlider.Value) * 100;
        }
        
        
        private void ChartCheck_OnChecked(object sender, RoutedEventArgs e)
        {
            _showChart = true;
        }
        
        
        private void ChartCheck_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _showChart = false;
        }
        
        private void CreateCanvas(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = new Cell(Canvas, width, height, x, y);
                    cell.CellStateChange += Cell_CellStateChange;
                    _cells2D[x, y] = cell;
                    _cells[x * height + y] = cell;
                }
            }
            LifeCount.Visibility = Visibility.Visible;
        }

        private void Cell_CellStateChange(Cell cell, Cell.State state)
        {
            int dying = 0;
            int living = 0;
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell current = _cells[i];
                if (current.CellState == Cell.State.Dying)
                {
                    dying++;
                }
                else if (current.CellState == Cell.State.Alive)
                {
                    living++;
                }
            }
            
            _alive = living;
            _dead = dying;
            
            LifeCount.Content = "Alive: " + _alive;
            DeadCount.Content = "Dying: " + _dead;

            if (!_isRunning && _alive > 0)
            {
                CreateBtn.Visibility = Visibility.Hidden;
                StartBtn.Visibility = Visibility.Visible;
            } else if (!_isRunning && _alive == 0)
            {
                CreateBtn.Visibility = Visibility.Hidden;
                StartBtn.Visibility = Visibility.Hidden;
            }
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            _days++;
            int[,] neighbours = new int[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    neighbours[x, y] = GetNeighbours(x, y);
                    
                    if (_cells2D[x, y].CellState == Cell.State.Alive)
                    {
                        if (neighbours[x, y] < 2 || neighbours[x, y] > 3)
                        {
                            _cells2D[x, y].DrawDie();
                        }
                    }
                    else if (_cells2D[x, y].CellState == Cell.State.NotAlive)
                    {
                        if (neighbours[x, y] == 3)
                        {
                            _cells2D[x, y].DrawBorn();
                        }
                    }
                    else if (_cells2D[x, y].CellState == Cell.State.Dying)
                    {
                        _cells2D[x, y].DrawNotAlive();
                    }
                }
            }
            DayCount.Content = "Day: " + _days;

            if (_showChart)
            {
                if (_alive > _maxNum)
                {
                    _maxNum = _alive;
                }
                if (_dead > _maxNum)
                {
                    _maxNum = _dead;
                }
                
                _statistics.Add(_days, new Chart.StatisticFrame(_days, _alive, _dead));
                _chart.Draw(_statistics, _maxNum);
            }
        }

        private int GetNeighbours(int x, int y)
        {
            int neighbourCount = 0;
                    
            int xAbove = x - 1;
            int xBelow = x + 1;
            int yAbove = y - 1;
            int yBelow = y + 1;

            if (xAbove < 0)
            {
                xAbove = _width - 1;
            }
            if (xBelow >= _width)
            {
                xBelow = 0;
            }
            if (yAbove < 0)
            {
                yAbove = _height - 1;
            }
            if (yBelow >= _height)
            {
                yBelow = 0;
            }
            
            if (_cells2D[xAbove, yAbove].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[x, yAbove].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[xAbove, y].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[xBelow, yBelow].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[x, yBelow].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[xBelow, y].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[xAbove, yBelow].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }
            if (_cells2D[xBelow, yAbove].CellState == Cell.State.Alive)
            {
                neighbourCount++;
            }

            return neighbourCount;
        }
    }
}