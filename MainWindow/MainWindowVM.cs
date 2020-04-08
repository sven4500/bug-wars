using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading; // DispatchetTimer
using System.Collections.ObjectModel; // ObservableCollection<>
using System.ComponentModel; // INotifyPropertyChanged
using BugWars.GameObjects; // IGameObject

namespace BugWars
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
        // объектам типа Shape. Периодически по таймеру эта коллекция будет
        // обновлятся значениями из модели.
        readonly private ObservableCollection<IGameObject> gameObjects = new ObservableCollection<IGameObject>();
        public ObservableCollection<IGameObject> GameObjects { get { return gameObjects; } }

        // Коллекция линий для рисования сетки игрового поля. Эта коллекция не
        // будет изменяться во время работы программы.
        readonly private ObservableCollection<IGameObject> gridLines = new ObservableCollection<IGameObject>();

        readonly private Config conf;

        public MainWindowVM(Config _conf)
        {
            conf = _conf;

            for (uint i = 0; i <= conf.MapWidth; ++i)
            {
                VerticalGridLine line = new VerticalGridLine(conf.MapWidth, conf.MapHeight, canvasWidth, canvasHeight);

                line.PosX = i;
                line.PosY = 0;

                gridLines.Add(line);
            }

            for (uint i = 0; i <= conf.MapHeight; ++i)
            {
                HorizontalGridLine line = new HorizontalGridLine(conf.MapWidth, conf.MapHeight, canvasWidth, canvasHeight);

                line.PosX = 0;
                line.PosY = i;

                gridLines.Add(line);
            }

            // Пример использования таймера:
            // https://qna.habr.com/q/76590
            timer = new DispatcherTimer();
            timer.Tick += Tick;
            timer.Interval = new TimeSpan((long)(10000000 * refreshRate));
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            gameObjects.Clear();
            gridLines.ToList().ForEach(gameObjects.Add);

            // Говорим WPF что коллекция игровых объектов изменилась и нужно
            // нарисовать их заново.
            OnPropertyChanged("GameObjects");
        }
    }
}
