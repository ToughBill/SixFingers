using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    class CommandsManager
    {

        private static CommandsManager _instance = null;
        /// <summary>
        /// Gets the single insntace of CommandsManager
        /// </summary>
        public static CommandsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommandsManager();
                }

                return _instance;
            }
        }
    }
}
