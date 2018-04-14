using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WTPipetting.Data;
using WorkstationController.Core.Data;

namespace WTPipetting.Utility
{
    class GlobalVars
    {
        private static GlobalVars _instance;
        private int _sampleCnt = 16;
        private string protocolName = "";
        
        public GlobalVars()
        {
           
        }
        
        static public GlobalVars Instance
        {
            get
            {
                if(_instance == null )
                {
                    _instance = new GlobalVars();
                }
                return _instance;
            }
        }
       

        public string ProtocolName
        {
            get
            {
                return protocolName;
            }
            set
            {
                protocolName = value;
            }
        }

        public int SampleCount 
        { 
            get 
            { 
                return _sampleCnt;
            }
            set 
            { 
                _sampleCnt = value;
            }
        }

        Layout _selectedLayout;
        public Layout selectedLayout
        {
            get 
            {
                return _selectedLayout;
            }
            set 
            {
                _selectedLayout = value;
            }

        }


        public Stage FarthestStage { get; set; }

   
        public string RecipeName { get; set; }

        public int StartGrid { get; set; }

        public Dictionary<CellPosition, SampleInfo> SampleLayoutSettings { get; set; }
    }
    
}
