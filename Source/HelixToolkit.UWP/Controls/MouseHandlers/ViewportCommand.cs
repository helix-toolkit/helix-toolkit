// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportCommand.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Windows.Foundation.Metadata;
    using Windows.UI.Xaml.Markup;

    /// <summary>
    /// An <see cref="ICommand"/> implementation that can be bound to an <see cref="InputGesture"/> in XAML using
    /// <see cref="InputBinding"/> derivations.
    /// </summary>
    [CreateFromString(MethodName = "CreateFromString")]
    public class ViewportCommand : ICommand
    {
        private static readonly Dictionary<ViewportCommands.Id, ViewportCommand> Commands = 
            new Dictionary<ViewportCommands.Id, ViewportCommand>();

        public event EventHandler CanExecuteChanged;

        public Action<object> ExecuteHandler { get; set; }

        public Func<object, bool> CanExecuteHandler { get; set; }

        public ViewportCommands.Id Id { get; }

        private ViewportCommand(ViewportCommands.Id id)
        {
            this.Id = id;
            Commands[id] = this;
        }

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteHandler?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            this.ExecuteHandler.Invoke(parameter);
        }

        public static ViewportCommand Get(ViewportCommands.Id id)
        {
            if (Enum.IsDefined(typeof(ViewportCommands.Id), id))
            {
                if (Commands.TryGetValue(id, out var value))
                {
                    return value;
                }

                return new ViewportCommand(id);
            }

            return null;
        }

        public static ViewportCommand CreateFromString(string value)
        {
            if (Enum.TryParse<ViewportCommands.Id>(value, false, out var id))
            {
                return Get(id);
            }

            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Markup.MarkupExtension" />
    public class ViewportCommandExtension : MarkupExtension
    {
        private readonly ViewportCommand value;
    
        public ViewportCommandExtension(ViewportCommand value)
        {
            this.value = value;
        }
    
        protected override object ProvideValue()
        {
            return this.value;
        }
    }
}
