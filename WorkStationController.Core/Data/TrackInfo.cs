using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Core.Data
{
    public class LihaTrackInfo
    {
        private string srcLabware;
        private string dstLabware;
        private string srcWellID;
        private string dstWellID;
        private double volume;
        private PipettingResult result;
        public LihaTrackInfo(string srcLabware, string dstLabware, 
            string srcWellID, string dstWellID,
            double volume ,PipettingResult result)
        {
            this.srcLabware = srcLabware;
            this.dstLabware = dstLabware;
            this.srcWellID = srcWellID;
            this.dstWellID = dstWellID;
            this.volume = volume;
            this.result = result;
        }

        public double Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        public PipettingResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        public string SrcWellID
        {
            get
            {
                return srcWellID;
            }
            set
            {
                srcWellID = value;
            }
        }

        public string DstWellID
        {
            get
            {
                return dstWellID;
            }
            set
            {
                dstWellID = value;
            }
        }

        public string SrcLabware
        {
            get
            {
                return srcLabware;
            }
            set
            {
                srcLabware = value;
            }
        }

        public string DstLabware
        {
            get
            {
                return dstLabware;
            }
            set
            {
                dstLabware = value;
            }
        }



    }

    public class RomaTrackInfo
    {
        private string srcLabware;
        private string dstLabware;
        public RomaTrackInfo(string srcLabware, string dstLabware )
        {
            this.srcLabware = srcLabware;
            this.dstLabware = dstLabware;
        }

        public string SrcLabware
        {
            get
            {
                return srcLabware;
            }
            set
            {
                srcLabware = value;
            }
        }

        public  string DstLabware
        {
            get
            {
                return dstLabware;
            }
            set
            {
                dstLabware = value;
            }
        }
    }


    public enum PipettingResult
    {
        ok,
        air,
        nothing,
        abort
    }
}
