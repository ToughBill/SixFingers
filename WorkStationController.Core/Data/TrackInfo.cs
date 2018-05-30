using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Core.Data
{
    public class PipettingTrackInfo : ITrackInfo
    {
        private string barcode;
        private string labware;
        private string wellID;
        private double volume;
        private PipettingResult result;
        bool isAsp;
        public PipettingTrackInfo(string labware, string wellID,
            double volume ,PipettingResult result,string barcode = "", bool isAsp = true)
        {
            this.labware = labware;
            this.wellID = wellID;
            this.volume = volume;
            this.result = result;
            this.isAsp = isAsp;
            this.barcode = barcode;
        }

        public string Barcode
        {
            get
            {
                return barcode;
            }
        }
        public string Name
        {
            get
            {
                return isAsp? "Aspirate" : "Dispense";
            }
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

        public string WellID
        {
            get
            {
                return wellID;
            }
            set
            {
                wellID = value;
            }
        }

     

        public string Labware
        {
            get
            {
                return labware;
            }
            set
            {
                labware = value;
            }
        }


        public string Stringfy()
        {
            
            if( isAsp)
                return string.Format("从{0}的孔{1}吸{2}ul,-{3}", Labware, WellID, Volume,TranslateResult(result));
            else
                return string.Format("喷{0}ul到{1}的孔{2}", Volume,Labware, WellID);
        }

        private string TranslateResult(PipettingResult result)
        {
            string s = "";
            switch(result)
            {
                case PipettingResult.abort:
                    s = "中止！";
                    break;
                case PipettingResult.air:
                    s = "吸空气";
                    break;
                case PipettingResult.nothing:
                    s = "跳过";
                    break;
                case PipettingResult.ok:
                    s = "";
                    break;
                case PipettingResult.zmax:
                    s = "zMax";
                    break;
            }
            return s;
        }
    }

    public class RomaTrackInfo : ITrackInfo
    {
        private string srcLabware;
        private string dstLabware;
        bool allFinished = true;
        public RomaTrackInfo(string srcLabware, string dstLabware, bool allFinished = true )
        {
            this.srcLabware = srcLabware;
            this.dstLabware = dstLabware;
            this.allFinished = allFinished;
        }
        public bool AllFinished
        {
            get
            {
                return allFinished;
            }
            set
            {
                allFinished = value;
            }
        }

        public string Name
        {
            get
            {
                return "Roma";
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


        public string Stringfy()
        {
            return string.Format("从{0}抓到{1}", srcLabware, dstLabware);
        }
    }

    public class DitiTrackInfo : ITrackInfo
    {
        private bool isGetDiti = true;
        public DitiTrackInfo(string label, int ditiID, bool succeed, bool isGetDiti = true)
        {
            DitiLabel = label;
            ID = ditiID;
            Succeed = succeed;
            this.isGetDiti = isGetDiti;
        }

        public bool Succeed { get; set; }
        public string DitiLabel { get; set; }
        public int ID { get; set; }
        public string Name
        {
            get
            {
                return isGetDiti ? "GetDiti" : "DropDiti";
            }
        }

        public string Stringfy()
        {
            return string.Format("从{0}的第{1}个位置取枪头,{2}", DitiLabel, ID,Succeed ? "成功":"失败");
        }
    }
    public interface ITrackInfo
    {
        string Name { get; }
        string Stringfy();
    }

    
    public enum PipettingResult
    {
        ok,
        air,
        clotIgnore,
        clotDropDiti,
        clotDispenseBack,
        bubble,
        nothing,
        zmax,
        abort
    }
}
