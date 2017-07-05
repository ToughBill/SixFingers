using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class PipettingCommand : IPipettorCommand
    {
        List<int> _tipsIDUsed;
        List<int> _selectedWellIDs;
        bool _isAspirate;
        int _volumeUL;
        string _labwareLabel;
        LiquidClass _liquidClass;
        
        public PipettingCommand(string labwareLabel,
            List<int> tipIDUsed,
            List<int> selectedWellIDs,
            LiquidClass liquidClass,
            bool isAspirate,
            int volumeUL)
        {
            
            _tipsIDUsed = tipIDUsed;
            _selectedWellIDs = selectedWellIDs;
            _isAspirate = isAspirate;
            _volumeUL = volumeUL;
            _labwareLabel = labwareLabel;
        }

        public int VolumeUL
        {
            get
            {
                return _volumeUL;
            }
        }

        public LiquidClass LiquidClass
        {
            get
            {
                return _liquidClass;
            }
        }

        public string LabwareLabel
        {
            get
            {
                return _labwareLabel;
            }
        }

        public List<int> SelectedWellIDs
        {
            get
            {
                return _selectedWellIDs;
            }
        }

        public List<int> TipsIDsUsed
        {
            get
            {
                return _tipsIDUsed;
            }
        }

        public bool IsAspirate
        {
            get
            {
                return _isAspirate;
            }
        }


        public string Name
        {
            get;
            set;
        }
    }
}
