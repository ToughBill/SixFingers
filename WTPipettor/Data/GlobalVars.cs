using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WTPipettor.Data;

namespace WTPipettor.Data
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

        //public SampleLayoutSettings SampleLayoutSettings { get; set; }


        public Stage FarthestStage { get; set; }

        public bool UseLastTimeSetting { get; set; }

        public string RecipeName { get; set; }
    }
    
}
