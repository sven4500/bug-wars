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
        private readonly MainWindowVM viewModel;

        public MainWindow(Config conf)
        {
            viewModel = new MainWindowVM(conf);
            DataContext = viewModel;
            InitializeComponent();
            viewModel.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Уничтожаем окно. Как уничтожить окно описано здесь:
            // https://stackoverflow.com/questions/568408/what-is-the-correct-way-to-dispose-of-a-wpf-window
            viewModel.Pause();
            this.Close();
        }
    }
}
