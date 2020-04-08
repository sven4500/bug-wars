using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data; // IValueConverter
using System.Globalization; // CultureInfo
using System.Collections.ObjectModel; // ObservableCollection
using System.Windows.Shapes; // Shape
using System.Windows.Media; // Brushes
using BugWars.GameObjects;

namespace BugWars
{
    public class GameObjectConverter : IValueConverter
    {
        private double gridOpacity = 1.0;
        public double GridOpacity { get { return gridOpacity; } set { gridOpacity = (value >= 0.0 && value <= 1.0) ? value : gridOpacity; } }

        public GameObjectConverter()
        { }

        private Shape ConvertHorizontalLine(IGameObject gameObject)
        {
            HorizontalGridLine gridLine = gameObject as HorizontalGridLine;
            if (gridLine == null)
                return null;

            Line line = new Line();

            line.X1 = 0;
            line.Y1 = gridLine.StepHeight * gridLine.PosY;

            line.X2 = gridLine.CanvasWidth;
            line.Y2 = line.Y1;

            line.Stroke = new SolidColorBrush(Colors.Black);
            line.Stroke.Opacity = GridOpacity;
            line.StrokeThickness = 1;

            return line;
        }

        private Shape ConvertVerticalLine(IGameObject gameObject)
        {
            VerticalGridLine gridLine = gameObject as VerticalGridLine;
            if (gridLine == null)
                return null;

            Line line = new Line();

            line.X1 = gridLine.StepWidth * gridLine.PosX;
            line.Y1 = 0;

            line.X2 = line.X1;
            line.Y2 = gridLine.CanvasHeight;

            line.Stroke = new SolidColorBrush(Colors.Black);
            line.Stroke.Opacity = GridOpacity;
            line.StrokeThickness = 1;

            return line;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<IGameObject> collection = value as ObservableCollection<IGameObject>;
            if (value == null || collection == null)
                return null;

            ObservableCollection<Shape> shapes = new ObservableCollection<Shape>();

            foreach (IGameObject gameObject in collection)
            {
                Shape shape = null;

                if (gameObject == null)
                {
                    shape = null;
                }
                else if (gameObject as HorizontalGridLine != null)
                {
                    shape = ConvertHorizontalLine(gameObject);
                }
                else if (gameObject as VerticalGridLine != null)
                {
                    shape = ConvertVerticalLine(gameObject);
                }
                else
                {
                    throw new NotSupportedException();
                }

                shapes.Add(shape);
            }

            return shapes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
