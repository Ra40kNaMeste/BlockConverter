using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlockConverter.ViewModels
{
    public class CustomCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action<object> Action { get; init; }
        private Func<object, bool> Func { get; init; }
        public CustomCommand(Action<object> action, Func<object,bool> condition = null)
        {
            Action = action;
            Func = condition;

        }
        public bool CanExecute(object parameter)
        {
            return Func == null || Func.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            Action?.Invoke(parameter);
        }
    }
}
