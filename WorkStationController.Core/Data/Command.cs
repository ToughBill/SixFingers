using System;
using System.Collections.Generic;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Command definition
    /// </summary>
    [Serializable]
    public class Command : ICloneable
    {
        private string _name       = string.Empty;
        private string _parameters = string.Empty;

        /// <summary>
        /// Create the supported command list
        /// </summary>
        /// <returns>The supported command list</returns>
        public static List<string> CreatSupportedCommands()
        {
            List<string> commands = new List<string>();
            
            commands.Add(Aspirate.Name);
            commands.Add(Dispense.Name);
            //commands.Add(new Command("Get DiTi", ""));
            //commands.Add(new Command("Drop DiTi", ""));
            return commands;
        }

        /// <summary>
        /// Default constructor for deserialization
        /// </summary>
        public Command()
        { }

        public Command(string name, string parameters)
        {
            this._name = name;
            this._parameters = parameters;
        }

        /// <summary>
        /// Gets or sets the name of the command
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Gets or sets the parameters of the command
        /// </summary>
        public string Parameters
        {
            get { return this._parameters; }
            set { this._parameters = value; }
        }

        public override string ToString()
        {
            return this._name;
        }

        public object Clone()
        {
            return new Command(this._name, this._parameters);
        }
    }
}
