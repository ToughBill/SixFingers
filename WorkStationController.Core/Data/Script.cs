using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Script definition, that is a serise of Command instances.
    /// </summary>
    [Serializable]
    public class Script : ObservableCollection<Command>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Script() 
        { }

        /// <summary>
        /// Inserts an command into the script at the specified index
        /// </summary>
        /// <param name="index">The zero-based index at which command should be inserted.</param>
        /// <param name="command">The command to insert</param>
        public void InsertCommand(int index, Command command)
        {
            this.Insert(index, command);
        }

        /// <summary>
        /// Moves the command at the specified index to a new location
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the command to be moved.</param>
        /// <param name="newIdex">The zero-based index specifying the new location of the command.</param>
        public void MoveCommand(int oldIndex, int newIdex)
        {
            this.MoveItem(oldIndex, newIdex);
        }

        /// <summary>
        /// Delete a command at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the command to remove.</param>
        public void DeleteCommand(int index)
        {
            this.RemoveItem(index);
        }
    }
}
