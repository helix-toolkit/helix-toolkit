using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileLoadDemo
{
  public class DelegateCommand : ICommand
  {
    private Action execute;

    public void Dispose()
    {
      this.execute = null;
      this.canExecute = null;
    }
    private Func<bool> canExecute;

    public DelegateCommand(Action execute)
      : this(execute, null)
    {
    }

    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }

      this.execute = execute;
      this.canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (canExecute != null)
        {
          CommandManager.RequerySuggested += value;
        }
      }

      remove
      {
        if (canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
        }
      }
    }

    public void RaiseCanExecuteChanged()
    {
      CommandManager.InvalidateRequerySuggested();
    }

    public bool CanExecute(object parameter)
    {
      return canExecute == null ? true : canExecute();
    }

    public void Execute(object parameter)
    {
      execute();
    }
  }
}
