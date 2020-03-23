using System;
using System.Collections.Generic;
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

namespace Course
{
    public partial class MainWindow : Window
    {
        // TODO: хздесь немного криво получается потому что нужно править класс
        // view model в двух местах. Здесь и в XAML схеме.
        private MainWindowVM ViewModel { get { return DataContext as MainWindowVM; } }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
