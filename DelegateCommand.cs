using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // ICommand

namespace BugWars
{
    // Если использовать шаблон проектирования MMVM, то следует действие кнопки
    // свяхывать с делегатами внутри view model.
    // https://stackoverflow.com/questions/37972561/why-use-commands-in-wpf-and-not-event-handlers
    // https://social.technet.microsoft.com/wiki/contents/articles/18199.event-handling-in-an-mvvm-wpf-application.aspx

    // Идея этого класса заключается в том чтобы создать своего рода
    // универсальный транслятор команд в пользовательские функции.
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _action;

        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

    }
}
