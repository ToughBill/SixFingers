using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkStationController.Core.Data
{
    public class Configurations
    {
        static Configurations instance = null;
        static public Configurations Instance
        {
            get
            {
                if (instance == null)
                    instance = new Configurations();
                return instance;
            }
        }
        private Configurations()
        {

        }

        private List<ArmInfo> armsInfo;
        private Worktable tableSurface;
        public int pumpComPort;
        
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

    }

    public class ArmInfo
    {
        public byte ID;
        public byte address;
        private List<TipType> tipsType;
        private List<PumpInfo> pumpsInfo;
        public int TipCount 
        { 
            get
            {
                return tipsType.Count;
            }
        }

        public List<TipType> TipsType
        { 
            get
            {
                return tipsType;
            }
            set
            {
                tipsType = value;
            }
        }

        public List<PumpInfo> PumpsInfo
        {
            get
            {
                return pumpsInfo;
            }
            set
            {
                pumpsInfo = value;
            }
        }
    }
    public class TipInfo
    {
        public TipType tipType;
        public double DilutorCapacity;
    }

    public enum TipType
    {
        Fixed = 0,
        Disposable = 1,
    }
}
