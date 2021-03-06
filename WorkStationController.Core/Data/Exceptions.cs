﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Core.Data
{
    public class NoEngouhDitiException : Exception
    {
        public LabwareTrait LabwareTrait { get; set; }
        public int NeedTipCount { get; set; }
        public NoEngouhDitiException(LabwareTrait labwareTrait, int needTipCnt)
        {
            LabwareTrait = labwareTrait;
            NeedTipCount = needTipCnt;
        }

    }

    public class NoDitiBoxException : Exception
    {
        public DitiType DitiType { get; set; }

        public NoDitiBoxException(DitiType ditiType)
        {
            this.DitiType = ditiType;
        }

    }


    public class SkipException : Exception
    {
    }
    public class CriticalException : Exception
    {
        public string Description { get; set; }
        public CriticalException(string desc)
        {
            Description = desc;
        }
    }


    public class WellOfoutRange:Exception
    {
        public string LabwareLabel { get; set; }
        public int MaxAllowedWell { get;set; }
        public WellOfoutRange(string label, int maxWellCnt)
        {
            LabwareLabel = label;
            MaxAllowedWell = maxWellCnt;
        }
    }

    public class NoLabwareException : Exception
    {
        public string Label { get; set; }
        public NoLabwareException(string label)
        {
            Label = label;
        }
    }
}
