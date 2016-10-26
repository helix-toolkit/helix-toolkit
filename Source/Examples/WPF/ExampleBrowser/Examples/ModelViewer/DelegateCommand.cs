// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ModelViewer
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private readonly Action execute;

        private readonly Func<bool> canExecute;

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