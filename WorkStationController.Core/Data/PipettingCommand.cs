using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class AdvancedPipettingCommand : IPipettorCommand
    {
        List<int> _tipsIDUsed;
        List<int> _selectedWellIDs;
        bool _isAspirate;
        float _volumeUL;
        string _labwareLabel;
        LiquidClass _liquidClass;
        
        public AdvancedPipettingCommand(string labwareLabel,
            List<int> tipIDUsed,
            List<int> selectedWellIDs,
            LiquidClass liquidClass,
            bool isAspirate,
            float volumeUL)
        {
            Name = "AdvPipettingCommand";
            _tipsIDUsed = tipIDUsed;
            _selectedWellIDs = selectedWellIDs;
            _isAspirate = isAspirate;
            _volumeUL = volumeUL;
            _labwareLabel = labwareLabel;
        }

        public float VolumeUL
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

    public class BasicPipettingCommand : IPipettorCommand
    {
        bool _isAspirate;
        float _volumeUL;
        string _labwareLabel;
        int _wellID;


        public BasicPipettingCommand(string labware, int wellID, float volume, bool isAsp )
        {
            _labwareLabel = labware;
            _wellID = wellID;
            _volumeUL = volume;
            _isAspirate = isAsp;
        }
        public string Name
        {
            get;set;
        }
    }
}
