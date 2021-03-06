﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Configurations
    /// </summary>
    public class Configurations
    {
        /// <summary>
        /// Single instance of Configurations
        /// </summary>
        static private Configurations instance = null;

        /// <summary>
        /// Gets the single instance of Configurations
        /// </summary>
        static public Configurations Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configurations();
                    
                }
                return instance;
            }
        }

        /// <summary>
        /// Arms collection
        /// </summary>
        private List<ArmInfo> armsInfo = null;

        /// <summary>
        /// Worktable instance
        /// </summary>
        public Worktable Worktable { get; set; }
        
        /// <summary>
        /// Gets or sets the pump com port
        /// </summary>
        public int PumpComPort { get; set; }

        /// <summary>
        /// Gets or sets the arms collection
        /// </summary>
        public List<ArmInfo> ArmsInfo
        {
            get
            {
                return armsInfo;
            }
            set
            {
                armsInfo = value;
            }
        }

        /// <summary>
        /// Private default constructor
        /// </summary>
        private Configurations()
        {
            Worktable = new Worktable(
                                     new Size(1000, 350),
                                     new Size(6, 6),
                                     new Point(0, 0), 250, 38);
        }
    }
}
