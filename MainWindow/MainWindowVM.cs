using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading; // DispatchetTimer
using System.Collections.ObjectModel; // ObservableCollection<>
using BugWars.GameObjects; // IGameObject

namespace BugWars
{
    public class MainWindowVM
    {
        readonly private MainWindowModel model = new MainWindowModel();

        private uint canvasWidth = 640;
        public uint CanvasWidth { get { return canvasWidth; } }

        private uint canvasHeight = 480;
        public uint CanvasHeight { get { return canvasHeight; } }

        private double refreshRate = 1.0;
        public double RefreshRate { get { return refreshRate; } }

        // Таймер который срабатывает 1 раз/сек и обновляет модель игры.
        readonly private DispatcherTimer timer;

        // В этой коллекции находится абсолютно всё что должен нарисовать
        // Canvas. Для объекта Canvas добавлен преобразователь данных который
        // самостоятельно производит преобразование GameObject объектов к
        // объектам типа Shape.
        readonly private ObservableCollection<IGameObject> gameObjects = new ObservableCollection<IGameObject>();
        public ObservableCollection<IGameObject> GameObjects { get { return gameObjects; } }

        readonly private Config conf;

        public MainWindowVM(Config _conf)
        {
            conf = _conf;

            makeGrid();

            // Пример использования таймера:
            // https://qna.habr.com/q/76590
            timer = new DispatcherTimer();
            timer.Tick += Tick;
            timer.Interval = new TimeSpan((long)(10000000 * refreshRate));
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        { }

        private void makeGrid()
        {
            for (uint i = 0; i <= conf.MapWidth; ++i)
            {
                VerticalGridLine line = new VerticalGridLine();

                line.CanvasWidth = canvasWidth;
                line.CanvasHeight = canvasHeight;
                
                line.MapWidth = conf.MapWidth;
                line.MapHeight = conf.MapHeight;

                line.PosX = i;
                line.PosY = 0;

                gameObjects.Add(line);
            }

            for (uint i = 0; i <= conf.MapHeight; ++i)
            {
                HorizontalGridLine line = new HorizontalGridLine();

                line.CanvasWidth = canvasWidth;
                line.CanvasHeight = canvasHeight;

                line.MapWidth = conf.MapWidth;
                line.MapHeight = conf.MapHeight;

                line.PosX = 0;
                line.PosY = i;

                gameObjects.Add(line);
            }
        }
    }
}
