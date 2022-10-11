using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife;

public class Cell
{
    private readonly Ellipse _ellipse;
    private bool _gameStarted;
    
    public int X { get; set; }
    public int Y { get; set; }
    
    public State CellState { get; set; }
    
    public enum State
    {
        Unselected,
        Alive,
        Dying,
        NotAlive
    }
    
    public delegate void CellStateChangeHandler(Cell cell, State state);
    public event CellStateChangeHandler CellStateChange;
    
    public Ellipse Ellipse
    {
        get { return _ellipse; }
    }
    
    public Cell(Canvas canvas, int gameWidth, int gameHeight, int x, int y)
    {
        X = x;
        Y = y;
        CellState = State.Unselected;
        
        _gameStarted = false;
        _ellipse = new Ellipse();
        _ellipse.Width = canvas.ActualWidth / gameWidth - 1;
        _ellipse.Height = canvas.ActualHeight / gameHeight - 1;
        _ellipse.Fill = Brushes.Cyan;
        _ellipse.DataContext = this;
        _ellipse.MouseEnter += ColorHover;
        _ellipse.MouseLeave += ColorNormal;
        _ellipse.MouseLeftButtonDown += SelectCell;
        Canvas.SetLeft(_ellipse, x * canvas.ActualWidth / gameWidth);
        Canvas.SetTop(_ellipse, y * canvas.ActualHeight / gameHeight);
        canvas.Children.Add(_ellipse);
    }
    
    private void SelectCell(object sender, MouseButtonEventArgs e)
    {
        Ellipse cell = (Ellipse)sender;
        Cell cellData = (Cell)cell.DataContext;
        if (!_gameStarted)
        {
            if (cellData.CellState == State.Unselected)
            {
                CellSelect(cellData, cell);
            }
            else
            {
                CellUnselect(cellData, cell);
            }
        }
    }

    public void CellSelect(Cell cell, Ellipse body)
    {
        cell.CellState = State.Alive;
        body.Fill = Brushes.LimeGreen;
        CellStateChange(cell, cell.CellState);
    }
    
    public void CellUnselect(Cell cell, Ellipse body)
    {
        cell.CellState = State.Unselected;
        body.Fill = Brushes.Cyan;
        CellStateChange(cell, cell.CellState);
    }

    public void DrawStartSimulation()
    {
        _gameStarted = true;
        _ellipse.MouseLeftButtonDown -= SelectCell;
        _ellipse.MouseEnter -= ColorHover;
        _ellipse.MouseLeave -= ColorNormal;
        if (CellState == State.Unselected)
        {
            _ellipse.Fill = Brushes.Transparent;
            CellState = State.NotAlive;
        }
    }
    
    public void DrawDie()
    {
        CellState = State.Dying;
        _ellipse.Fill = Brushes.Red;
        CellStateChange(this, CellState);
    }
    
    public void DrawNotAlive()
    {
        CellState = State.NotAlive;
        _ellipse.Fill = Brushes.Transparent;
        CellStateChange(this, CellState);
    }
    
    public void DrawBorn()
    {
        CellState = State.Alive;
        _ellipse.Fill = Brushes.LimeGreen;
        CellStateChange(this, CellState);
    }
    
    private void ColorHover(object sender, MouseEventArgs e)
    {
        Ellipse ellipse = (Ellipse)sender;
        Cell cell = (Cell)ellipse.DataContext;
        if (cell.CellState == State.Unselected)
        {
            ellipse.Fill = Brushes.DodgerBlue;
        } else if (cell.CellState == State.Alive)
        {
            ellipse.Fill = Brushes.ForestGreen;
        }
    }

    private void ColorNormal(object sender, MouseEventArgs e) {
        Ellipse ellipse = (Ellipse)sender;
        Cell cell = (Cell)ellipse.DataContext;
        if (cell.CellState == State.Unselected)
        {
            ellipse.Fill = Brushes.Cyan;
        } else if (cell.CellState == State.Alive)
        {
            ellipse.Fill = Brushes.LimeGreen;
        }
    }
}