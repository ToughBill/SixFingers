using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTPipetting.Data
{
     class PipettingInfo
    {
        public string srcLabware;
        public int srcWellID;
        public string dstLabware;
        public int dstWellID;
        public double vol;

        public PipettingInfo(string srcLabware,
            int srcWell, string dstLabware, int dstWell, double v)
        {
            this.srcLabware = srcLabware;
            this.dstLabware = dstLabware;
            this.srcWellID = srcWell;
            this.dstWellID = dstWell;
            this.vol = v;
        }

        public PipettingInfo(PipettingInfo pipettingInfo)
        {
           
            srcLabware = pipettingInfo.srcLabware;
            dstLabware = pipettingInfo.dstLabware;
            srcWellID = pipettingInfo.srcWellID;
            dstWellID = pipettingInfo.dstWellID;
            vol = pipettingInfo.vol;
        }
    }
