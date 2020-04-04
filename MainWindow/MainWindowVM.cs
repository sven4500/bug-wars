using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes; // Shape
using System.Windows.Media; // Brushes
using System.Collections.ObjectModel; // ObservableCollection<>

namespace BugWars
{
    public class MainWindowVM
    {
        readonly private MainWindowModel model = new MainWindowModel();

        private uint canvasWidth = 500;
        public uint CanvasWidth { get { return canvasWidth; } }

        private uint canvasHeight = 500;
        public uint CanvasHeight { get { return canvasHeight; } }

        // В этой коллекции находится абсолютно всё что должно нарисовать Canvas.
        readonly private ObservableCollection<Shape> shapes = new ObservableCollection<Shape>();
        public ObservableCollection<Shape> Shapes { get { return shapes; } }

        readonly private Config conf;

        public MainWindowVM(Config _conf)
        {
            conf = _conf;

            for(uint i = 0; i <= _conf.MapWidth; ++i)
            {
                Line line = new Line();
                
                line.X1 = (double)CanvasWidth / conf.MapWidth * i;
                line.Y1 = 0;

                line.X2 = line.X1;
                line.Y2 = CanvasHeight;

                line.Stroke = Brushes.Black;
                line.StrokeThickness = 1;

                shapes.Add(line);
            }

            for (uint i = 0; i <= _conf.MapHeight; ++i)
            {
                Line line = new Line();

                line.X1 = 0;
                line.Y1 = (double)CanvasHeight / conf.MapHeight * i;

                line.X2 = canvasWidth;
                line.Y2 = line.Y1;

                line.Stroke = Brushes.Black;
                line.StrokeThickness = 1;

                shapes.Add(line);
            }
        }
    }
}
