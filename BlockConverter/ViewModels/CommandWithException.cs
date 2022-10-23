using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlockConverter.ViewModels
{
    class CommandWithException : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action<object> Action { get; init; }
        private Func<object, bool> Func { get; init; }
        private Action<Exception> NoSuccessfullyAction { get; set; }
        private Action SuccessfullyAction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">комманда для выполнения</param>
        /// <param name="successfullyAction">комманда для вызова, если возникнет ошибка</param>
        /// <param name="condition">Условие для выполнения комманды</param>
        public CommandWithException(Action<object> action, Action successfullyAction = null, Action<Exception> nosuccessfullyAction = null, Func<object, bool> condition = null)
        {
            Action = action;
            NoSuccessfullyAction = nosuccessfullyAction;
            SuccessfullyAction = successfullyAction;
            Func = condition;
        }
        public bool CanExecute(object parameter)
        {

            return Func == null || Func.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                Action?.Invoke(parameter);
                SuccessfullyAction?.Invoke();
            }
            catch (Exception ex)
            {
                NoSuccessfullyAction?.Invoke(ex);
            }
        }
    }
}
