using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // ICommand
using System.Windows.Controls; // Canvas
using System.Windows.Threading; // DispatcherTimer
using System.Windows.Shapes; // Shape
using System.Windows.Media; // Brushes
using System.Windows.Media.Imaging; // BitmapImage
using System.Collections.ObjectModel; // ObservableCollection<>
using System.ComponentModel; // INotifyPropertyChanged
using BugWars.GameObjects; // IGameObject, Bug, Egg, Food, ..

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

        readonly private MainWindowModel model;
        readonly private Config conf;

        private uint canvasWidth = 640;
        public uint CanvasWidth
        {
            get { return canvasWidth; }
        }

        private uint canvasHeight = 480;
        public uint CanvasHeight
        {
            get { return canvasHeight; }
        }

        public double StepWidth
        {
            get { return (double)CanvasWidth / conf.MapWidth; }
        }

        public double StepHeight
        {
            get { return (double)CanvasHeight / conf.MapHeight; }
        }

        // Здесь относительно того нужна ли команда на ValueChanged событие:
        // https://stackoverflow.com/questions/25138695/how-to-handle-the-slider-valuechanged-event-in-a-view-model
        private double refreshRate = 1.0;
        public double RefreshRate
        {
            get { return refreshRate; }
            set { refreshRate = value; timer.Interval = new TimeSpan((long)(10000000 * refreshRate)); OnPropertyChanged("RefreshRate"); }
        }

        private double gridOpacity = 0.1;
        public double GridOpacity
        {
            get { return gridOpacity; }
            set { gridOpacity = (value >= 0.0 && value <= 1.0) ? value : gridOpacity; }
        }

        // Здесь относительно того как добавить ресурсы в проект:
        // https://stackoverflow.com/questions/13535587/how-to-create-imagebrush-in-c-sharp-code
        // https://wpf.2000things.com/2014/07/03/1107-accessing-an-embedded-resource-using-a-uri/
        private readonly ImageBrush bugMaleBlueBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/bug-male-blue.png")));
        private readonly ImageBrush bugFemaleBlueBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/bug-female-blue.png")));

        private readonly ImageBrush bugMaleRedBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/bug-male-red.png")));
        private readonly ImageBrush bugFemaleRedBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/bug-female-red.png")));

        private readonly ImageBrush eggBlueBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/egg-blue.png")));
        private readonly ImageBrush eggRedBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/egg-red.png")));

        private readonly ImageBrush crumbsBrush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/crumbs.png")));

        // Таймер который обновляет модель игры.
        readonly private DispatcherTimer timer;

        readonly private ICommand onPauseCommand;
        public ICommand OnPauseCommand
        {
            get { return onPauseCommand; }
        }

        readonly private ICommand onResumeCommand;
        public ICommand OnResumeCommand
        {
            get { return onResumeCommand; }
        }

        // В этой коллекции находится абсолютно всё что должен нарисовать
        // Canvas. Периодически по таймеру эта коллекция будет обновлятся
        // значениями из модели.
        readonly private ObservableCollection<Shape> shapes = new ObservableCollection<Shape>();
        public ObservableCollection<Shape> Shapes
        {
            get { return shapes; }
        }

        // Коллекция линий для рисования сетки игрового поля. Эта коллекция не
        // будет изменяться во время работы программы.
        readonly private ObservableCollection<Shape> gridLines = new ObservableCollection<Shape>();

        public MainWindowVM(Config _conf)
        {
            conf = _conf;
            model = new MainWindowModel(conf);

            for (uint i = 0; i <= conf.MapWidth; ++i)
            {
                Line line = new Line();

                line.X1 = StepWidth * i;
                line.Y1 = 0;

                line.X2 = line.X1;
                line.Y2 = CanvasHeight;

                line.Stroke = new SolidColorBrush(Colors.Black);
                line.Stroke.Opacity = GridOpacity;
                line.StrokeThickness = 1;

                gridLines.Add(line);
            }

            for (uint i = 0; i <= conf.MapHeight; ++i)
            {
                Line line = new Line();

                line.X1 = 0;
                line.Y1 = StepHeight * i;

                line.X2 = CanvasWidth;
                line.Y2 = line.Y1;

                line.Stroke = new SolidColorBrush(Colors.Black);
                line.Stroke.Opacity = GridOpacity;
                line.StrokeThickness = 1;

                gridLines.Add(line);
            }

            onPauseCommand = new DelegateCommand(Pause);
            onResumeCommand = new DelegateCommand(Start);

            // Пример использования таймера:
            // https://qna.habr.com/q/76590
            timer = new DispatcherTimer();
            timer.Tick += Tick;

            // Нужно явно задать RefreshRate чтобы проинициализировать таймер.
            RefreshRate = 1.0;
        }

        public void Start()
        {
            Start(null);
        }

        public void Pause()
        {
            Pause(this);
        }

        private void Start(object e)
        {
            timer.Start();
        }

        private void Pause(object e)
        {
            timer.Stop();
        }

        private Shape ConvertBug(IGameObject gameObject)
        {
            Bug bug = gameObject as Bug;

            if (bug == null)
            {
                return null;
            }

            Shape rect = new Rectangle();

            // Здесь относительно того как двигать фигуру программно:
            // https://stackoverflow.com/questions/23385876/moving-the-dynamically-drawn-rectangle-inside-the-canvas-using-mousemove-event
            rect.SetValue(Canvas.LeftProperty, bug.PosX * StepWidth);
            rect.SetValue(Canvas.TopProperty, bug.PosY * StepHeight);

            rect.Width = StepWidth;
            rect.Height = StepHeight;

            rect.Fill = (bug.Team == Bug.TeamEnum.Blue) ?
                ((bug.Sex == Bug.SexEnum.Male) ? bugMaleBlueBrush : bugFemaleBlueBrush) :
                ((bug.Sex == Bug.SexEnum.Male) ? bugMaleRedBrush : bugFemaleRedBrush);

            return rect;
        }

        private Shape ConvertEgg(IGameObject gameObject)
        {
            Egg egg = gameObject as Egg;

            if (egg == null)
            {
                return null;
            }

            Shape rect = new Rectangle();

            rect.SetValue(Canvas.LeftProperty, egg.PosX * StepWidth);
            rect.SetValue(Canvas.TopProperty, egg.PosY * StepHeight);

            rect.Width = StepWidth;
            rect.Height = StepHeight;

            rect.Fill = (egg.Team == Bug.TeamEnum.Blue) ?
                (eggBlueBrush) :
                (eggRedBrush);

            return rect;
        }

        private Shape ConvertCrumb(IGameObject gameObject)
        {
            Crumbs crumb = gameObject as Crumbs;

            if (crumb == null)
            {
                return null;
            }

            Shape shape = new Rectangle();

            shape.SetValue(Canvas.LeftProperty, crumb.PosX * StepWidth);
            shape.SetValue(Canvas.TopProperty, crumb.PosY * StepHeight);

            shape.Width = StepWidth;
            shape.Height = StepHeight;

            shape.Fill = crumbsBrush;

            return shape;
        }

        private Shape Convert(IGameObject gameObject)
        {
            Shape shape = null;

            if (gameObject == null)
            {
                //shape == null;
            }
            else if (gameObject as Bug != null)
            {
                shape = ConvertBug(gameObject);
            }
            else if (gameObject as Egg != null)
            {
                shape = ConvertEgg(gameObject);
            }
            else if (gameObject as Crumbs != null)
            {
                shape = ConvertCrumb(gameObject);
            }
            else
            {
                throw new NotSupportedException();
            }

            return shape;
        }

        private void AttachGameObjects<T>(IEnumerable<T> list) where T : class
        {
            foreach (var obj in list)
                Shapes.Add(Convert(obj as IGameObject));
        }

        private void AttachShapes(IEnumerable<Shape> list)
        {
            foreach (var obj in list)
                Shapes.Add(obj);
        }

        private void UpdateShapes()
        {
            Shapes.Clear();
            AttachShapes(gridLines);
            AttachGameObjects(model.BugsBlue);
            AttachGameObjects(model.BugsRed);
            AttachGameObjects(model.Eggs);
            AttachGameObjects(model.Crumbs);
        }

        private void Tick(object sender, EventArgs e)
        {
            model.Update();
            UpdateShapes();

            // Говорим WPF что коллекция объектов для отрисовки изменилась.
            OnPropertyChanged("Shapes");
        }
    }
}
