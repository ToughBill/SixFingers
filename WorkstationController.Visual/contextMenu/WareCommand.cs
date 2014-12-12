using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WorkstationController.VisualElement.contextMenu
{
    public class WareCommand : RoutedCommand
    {
        /// <summary>
        /// Occurs when the command initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        public new event CanExecuteRoutedEventHandler CanExecute
        {
            add
            {
                if (commandBinding == null)
                {
                    commandBinding = new CommandBinding(this, executed, value);
                    CommandManager.RegisterClassCommandBinding(OwnerType, commandBinding);
                }
                else
                {
                    commandBinding.CanExecute -= value;
                    commandBinding.CanExecute += value;
                }
                canExecute = value;
            }
            remove
            {
                if (commandBinding != null)
                {
                    commandBinding.CanExecute -= value;
                }
                canExecute = null;
            }
        }

        /// <summary>
        /// Occurs when the command executes.
        /// </summary>
        public event ExecutedRoutedEventHandler Executed
        {
            add
            {
                if (commandBinding == null)
                {
                    commandBinding = new CommandBinding(this, value, canExecute);
                    CommandManager.RegisterClassCommandBinding(OwnerType, commandBinding);
                }
                else
                {
                    commandBinding.Executed -= value;
                    commandBinding.Executed += value;
                }
                executed = value;
            }
            remove
            {
                if (commandBinding != null)
                {
                    commandBinding.Executed -= value;
                }
                executed = null;
            }
        }

        private CommandBinding commandBinding;
        private ExecutedRoutedEventHandler executed;
        private CanExecuteRoutedEventHandler canExecute;

        /// <summary>
        /// Initializes a new instance of the RibbonRoutedCommand class.
        /// </summary>
        public WareCommand()
            : this(Guid.NewGuid().ToString(), typeof(Window), null)
        { }

        /// <summary>
        /// Initializes a new instance of the RibbonRoutedCommand class with the specified name and owner type.
        /// </summary>
        /// <param name="name">Declared name for serialization.</param>
        /// <param name="ownerType">The type which is registering the command.</param>
        public WareCommand(string name, Type ownerType)
            : this(name, ownerType, null)
        { }

        /// <summary>
        /// Initializes a new instance of the RibbonRoutedCommand class with the specified name, owner type, and collection of gestures.
        /// </summary>
        /// <param name="name">Declared name for serialization.</param>
        /// <param name="ownerType">The type that is registering the command.</param>
        /// <param name="inputGestures">Default input gestures associated with this command.</param>
        public WareCommand(string name, Type ownerType, InputGestureCollection inputGestures)
            : base(name, ownerType, inputGestures)
        { }

        /// <summary>
        /// Raises the CanExecute event.
        /// </summary>
        /// <param name="args">A CanExecuteRoutedEventArgs that contains the event data.</param>
        protected virtual void OnCanExecute(CanExecuteRoutedEventArgs args)
        {
            if (canExecute != null)
            {
                canExecute(this, args);
            }
        }

        /// <summary>
        /// Raises the CanExecute event.
        /// </summary>
        /// <param name="args">A ExecutedRoutedEventArgs that contains the event data.</param>
        protected virtual void OnExecuted(ExecutedRoutedEventArgs args)
        {
            if (executed != null)
            {
                executed(this, args);
            }
        }
    }
}
