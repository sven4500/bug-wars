using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BugWars
{
    public class MainWindowVM
    {
        readonly private MainWindowModel model = new MainWindowModel();

        public Config Conf { get; set; }

        public MainWindowVM()
        { }
    }
}
