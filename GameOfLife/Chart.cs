using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GameOfLife;

public class Chart
{
    private readonly Canvas _canvas;
    
    public Chart(Canvas canvas)
    {
        _canvas = canvas;
        InitAxis();
    }
    
    public void Hide()
    {
        _canvas.Visibility = Visibility.Hidden;
    }
    
    public void Show()
    {
        _canvas.Visibility = Visibility.Visible;
    }

    public void Draw(Dictionary<int, StatisticFrame> stats, int maxNum)
    {
        _canvas.Children.Clear();
        InitAxis();
        if (stats.Count == 0)
            return;
        
        int lastAlive = stats[0].Alive;
        int lastDead = stats[0].Dead;
        for (int d = 1; d < stats.Count; d++)
        {
            StatisticFrame stat = stats[d];
            int alive = stat.Alive;
            int dead = stat.Dead;
            _canvas.Children.Add(new Line
            {
                X1 = _canvas.Width * (d - 1) / (stats.Count + 2),
                Y1 = _canvas.Height - (_canvas.Height * lastAlive / (maxNum + 2)),
                X2 = _canvas.Width * d / (stats.Count + 2),
                Y2 = _canvas.Height - (_canvas.Height * alive / (maxNum + 2)),
                Stroke = System.Windows.Media.Brushes.LimeGreen
            });
            _canvas.Children.Add(new Line
            {
                X1 = _canvas.Width * (d - 1) / (stats.Count + 2),
                Y1 = _canvas.Height - (_canvas.Height * lastDead / (maxNum + 2)),
                X2 = _canvas.Width * d / (stats.Count + 2),
                Y2 = _canvas.Height - (_canvas.Height * dead / (maxNum + 2)),
                Stroke = System.Windows.Media.Brushes.Red
            });
            lastAlive = alive;
            lastDead = dead;
        }
    }

    private void InitAxis()
    {
        _canvas.Children.Clear();
        _canvas.Children.Add(new Line
        {
            X1 = 0,
            Y1 = 0,
            X2 = 0,
            Y2 = _canvas.Height,
            Stroke = System.Windows.Media.Brushes.Gray
        });
        _canvas.Children.Add(new Line
        {
            X1 = 0,
            Y1 = _canvas.Height,
            X2 = _canvas.Width,
            Y2 = _canvas.Height,
            Stroke = System.Windows.Media.Brushes.Gray
        });
    }

    public class StatisticFrame
    {
        public int Day { get; set; }
        public int Alive { get; set; }
        public int Dead { get; set; }
        
        public StatisticFrame(int day, int alive, int dead)
        {
            Day = day;
            Alive = alive;
            Dead = dead;
        }
    }
}