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
using System.Windows.Shapes;

namespace BugWars
{
    public partial class MainWindow : Window
    {
        readonly private MainWindowVM viewModel = new MainWindowVM();

        public MainWindow(Config conf)
        {
            viewModel.Conf = conf;
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
