using RSPCOMMSERVERLib;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    public class RSPCommands : IPipettingCommmands
    {

        public void GetDiti(int tipMask, string labwareLabel, DitiType ditiType, TryTimes tryTimes, int armID)
        {
            throw new NotImplementedException();
        }

        public void DropDiti(int tipMask, string wasteLabel, int armID)
        {
            throw new NotImplementedException();
        }

        public void SetDitiPosition(string labwareLabel, DitiType ditiType, int position)
        {
            throw new NotImplementedException();
        }

        public void Aspirate(int tipMask, string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass, int armID = 1)
        {

        }

        public void Dispense(int tipMask, string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass, int armID = 1)
        {

        }

    }

    

    public partial class RSPCommandsImpl
    {
        RSPCommClass RSPCommu = null;
        const string start = "Start";
        const string top = "Top";
        const string cutoff = "CutOff";
        Dictionary<byte, byte> armID_Address = null;
        Dictionary<byte,Dictionary<byte, byte>> pumpID_AddressOfEachArm = null;
        
        public RSPCommandsImpl(RSPCommClass RSPCommu)
        {
            this.RSPCommu = RSPCommu;
            this.RSPCommu.RSPExitComm();
            armID_Address = GetArmIDAddressDict();
            pumpID_AddressOfEachArm = GetPumpIDAddressDictOfEachArm();
        }

        private Dictionary<byte, Dictionary<byte, byte>> GetPumpIDAddressDictOfEachArm()
        {
            Dictionary<byte, Dictionary<byte, byte>> pumpID_AddressDictOfEachArm
                = new Dictionary<byte, Dictionary<byte, byte>>();
            foreach (var armInfo in Configurations.Instance.ArmsInfo)
            {
                pumpID_AddressDictOfEachArm.Add(armInfo.ID, GetPumpIDAddress(armInfo));
            }
            return pumpID_AddressDictOfEachArm;
        }

        private Dictionary<byte, byte> GetPumpIDAddress(ArmInfo armInfo)
        {
            Dictionary<byte, byte> pumpIDAddress = armInfo.PumpsInfo.ToDictionary(x => x.ID, x=>x.Address);
            return pumpIDAddress;
        }

        private Dictionary<byte, byte> GetArmIDAddressDict()
        {
            return Configurations.Instance.ArmsInfo.ToDictionary(x => x.ID, x => x.Address);
        }

        public void Close()
        {
            this.RSPCommu.RSPExitComm();
        }
        #region helperCommands
        string SendCommand(string sCommand, byte armID = 1, byte deviceAddress = 8)
        {
            string sAnswer = "";
            byte armAddress = armID_Address[armID];
            sCommand = string.Format("{0}{1}{2}", armAddress, deviceAddress, sCommand);
            RSPCommu.RSPSendCommand(sCommand, armAddress, deviceAddress, ref sAnswer);
            return sAnswer;
        }

        void SendPumpCommand(string sCommand, byte armID = 1, byte pumpID = 1)
        {
            sCommand += "R";
            byte armAddress = armID_Address[armID];
            byte deviceAddress = pumpID_AddressOfEachArm[armID][pumpID];
            SendCommand(sCommand,armAddress,deviceAddress);
        }

        void SendCommand(string op, string commandDesc, List<int> parameters, byte armAddress = 1, byte deviceAddress = 8)
        {
            string ret = SendCommand(op, parameters, armAddress, deviceAddress);
            if (ret != "")
                throw new Exception("Failed to" + commandDesc + ret);
        }
        string SendCommand(string op, List<int> parameters, byte armAddress = 1, byte deviceAddress = 8)
        {
            string sCommand = op;
            for (int i = 0; i < parameters.Count; i++)
            {
                sCommand += " " + parameters[i].ToString();
                if (i != parameters.Count - 1)//not last
                    sCommand += ",";
            }
            return SendCommand(sCommand);
        }
        void SetOneVal(string op, string commandDesc, int val)
        {
            List<int> vals = new List<int>();
            vals.Add(val);
            string ret = SendCommand(op, vals);
            if (ret != "")
                throw new Exception("Failed to " + commandDesc + ret);
        }

        void SetXYZ(string op, string commandDesc, int x, int y, int z)
        {
            List<int> vals = new List<int>();
            vals.Add(x);
            vals.Add(y);
            vals.Add(z);
            SendCommand(op, commandDesc, vals);
        }

        #endregion

        #region SetCommands
        public void SetOverallMachineLimitation(int x, int y, int z)
        {
            string op = "OM";
            string commandDesc = strings.OM;
            SetXYZ(op, commandDesc, x, y, z);
        }

        public void SetRange4AbsoluteField(int x, int y, int z)
        {
            string op = "SM";
            string commandDesc = strings.SM;
            SetXYZ(op, commandDesc, x, y, z);
        }

        public void SetZParameters4AbsoluteField(int zMax, int zStart, int zDispense, int zTravel)
        {
            string op = "SA";
            string commandDesc = strings.SA;
            List<int> vals = new List<int>();
            vals.Add(zMax);
            vals.Add(zStart);
            vals.Add(zDispense);
            vals.Add(zTravel);
            SendCommand(op, commandDesc, vals);
        }

        public void SetNextXYMoveTravelHeight(int height)
        {
            string op = "SZ";
            string commandDesc = strings.SZ;
            SetOneVal(op, commandDesc, height);
        }

        public void SetLiquidSearchFrequency(int frequency)
        {
            string op = "SL";
            string commandDesc = strings.SZ;
            SetOneVal(op, commandDesc, frequency);
        }

        public void SetPositionRecovery(bool bOn)
        {
            int on = bOn ? 1 : 0;
            string op = "SP";
            string commandDesc = strings.SP;
            SetOneVal(op, commandDesc, on);
        }

        public void SetLiquidDetectionSensitivity(bool bHigh)
        {
            int high = bHigh ? 1 : 0;
            string op = "SS";
            string commandDesc = strings.SS;
            SetOneVal(op, commandDesc, high);
        }

        void SetRampParameters(string XYZ,int startFreq, int endFreq, int accel, int initFreq, int stepSize)
        {
            string op = XYZ;
            string commandDesc = strings.FX;
            switch(XYZ)
            {
                case "FY":
                    commandDesc = strings.FY;
                    break;
                case "FZ":
                    commandDesc = strings.FZ;
                    break;
            }
            List<int> vals = new List<int>();
            vals.Add(startFreq);
            vals.Add(endFreq);
            vals.Add(accel);
            vals.Add(initFreq);
            vals.Add(stepSize);
            SendCommand(op, commandDesc, vals);
        }

        public void SetYRampParameters(int startFreq, int endFreq, int accel, int initFreq, int stepSize)
        {
            SetRampParameters("FX", startFreq, endFreq, accel, initFreq, stepSize);
        }
        public void SetXRampParameters(int startFreq, int endFreq, int accel, int initFreq, int stepSize)
        {
            SetRampParameters("FY", startFreq, endFreq, accel, initFreq, stepSize);
        }
        public void SetZRampParameters(int startFreq, int endFreq, int accel, int initFreq, int stepSize)
        {
            SetRampParameters("FZ", startFreq, endFreq, accel, initFreq, stepSize);
        }

        public void SetDitiType(bool isFixedTip)
        {
            string op = "ST";
            int val = isFixedTip ? 0 : 1;
            SetOneVal(op, strings.ST, val);
        }

        public void SetSelfTestRange(int x, int y, int z)
        {
            string op = "OT";
            List<int> vals = new List<int>();
            vals.Add(x);
            vals.Add(y);
            vals.Add(z);
            string ret = SendCommand(op, vals);
            if (ret != "")
                throw new Exception("Failed to set self test range." + ret);
        }

        void SetInitOffSet(string XYZ, int val)
        {
            string op = string.Format("O{0}", XYZ);
            List<int> vals = new List<int>();
            vals.Add(val);
            string ret = SendCommand(op, vals);
            if (ret != "")
                throw new Exception(string.Format("Failed to set initial offset {0}.",XYZ) + ret);
        }

        public void SetInitOffSetX(int val)
        {
            SetInitOffSet("X", val);
        }

        public void SetInitOffSetY(int val)
        {
            SetInitOffSet("Y", val);
        }

        public void SetInitOffSetZ(int val)
        {
            SetInitOffSet("Z", val);
        }

        void AdjustOffset(string XYZ,int val)
        {
            string op = string.Format("{0}O", XYZ);
            List<int> vals = new List<int>();
            vals.Add(val);
            string ret = SendCommand(op, vals);
            if (ret != "")
                throw new Exception(string.Format("Failed to adjust offset {0}.", XYZ) + ret);
        }

        public void AdjustOffsetX(int val)
        {
            AdjustOffset("X", val);
        }
        public void AdjustOffsetY(int val)
        {
            AdjustOffset("Y", val);
        }
        public void AdjustOffsetZ(int val)
        {
            AdjustOffset("Z", val);
        }

        public void Write2EEPROM()
        {
            SendCommand("OW");
        }

        public void RestoreSettingFromEEPROM()
        {
            SendCommand("OR");
        }
        #endregion

        #region Arm Positioning Commands
        public void InitializePosition()
        {
            SendCommand("PI");
        }

        public void InitializePositionSimulation()
        {
            SendCommand("FI");
        }

        void InitializeOneAxis(string xyz, int speed = -1)
        {
            string op = string.Format("{0}I",xyz);
            if (speed == -1)
                SendCommand(op);
            else
            {
                List<int> vals = new List<int>();
                vals.Add(speed);
                SendCommand(op,vals);
                SendCommand(op);
            }
            
          
        }
        public void InitializeX(int speed = -1)
        {
            InitializeOneAxis("X", speed);
        }
        public void InitializeY(int speed = -1)
        {
            InitializeOneAxis("Y", speed);
        }
        public void InitializeZ(int speed = -1)
        {
            InitializeOneAxis("Z", speed);
        }

        public void Move2AbsolutePosition(int x,int y,int z)
        {
            string op = "PA";
            string sDesc = strings.PA;
            List<int> vals = new List<int>(){x,y,z};
            SendCommand(op, sDesc, vals);
        }

        public void MoveXAbs(int x)
        {
            string op = "XA";
            string sDesc = strings.XA;
            SetOneVal(op, sDesc, x);
        }
        public void MoveYAbs(int y)
        {
            string op = "YA";
            string sDesc = strings.YA;
            SetOneVal(op, sDesc, y);
        }
        public void MoveZAbs(int z)
        {
            string op = "ZA";
            string sDesc = strings.ZA;
            SetOneVal(op, sDesc, z);
        }

        public void MoveRelativeX(int x, int speed = -1)
        {
            string op = "XR";
            string sDesc = strings.XR;
            if (speed != -1)
            {
                op = "XS";
                SetOneVal(op, sDesc, speed);
            }
            else
                SendCommand(op);
        }
        
        public void MoveRelativeY(int y, int speed = -1)
        {
            string op = "YR";
            string sDesc = strings.YR;
            if (speed != -1)
            {
                op = "YS";
                SetOneVal(op, sDesc, speed);
            }
            else
                SendCommand(op);
        }
        
        public void MoveRelativeZ(int z, int speed = -1)
        {
            string op = "ZR";
            string sDesc = strings.ZR;
            if (speed != -1)
            {
                op = "ZS";
                SetOneVal(op, sDesc, speed);
            }
            else
                SendCommand(op);
        }
        #endregion
       
        #region Rack Definition Commands
        public void SetWastePosition(int ID, int x, int y, int z)
        {
            string op = "SW";
            string sDesc = strings.SW;
            List<int> vals = new List<int>() { ID, x, y, z };
            SendCommand(op, sDesc, vals);
        }

        public void SetPosition(int positionID, int x, int y, int z)
        {
            string op = "SF";
            string sDesc = strings.SF;
            List<int> vals = new List<int>() { positionID,x,y,z };
            SendCommand(op, sDesc, vals);
        }
        public void SetZParameters4Position(int positionID, int zMax, int zStart, int zDispense)
        {
            string op = "SR";
            string sDesc = strings.SR;
            List<int> vals = new List<int>() { positionID, zMax, zStart, zDispense };
            SendCommand(op, sDesc, vals);
        }
        #endregion

        #region Rack Positioning Commands
        public void Move2PredefinedPosition(int positionID)
        {
            if (positionID > 50 || positionID < 1)
                throw new Exception("predefined position ID must between 1 and 50!");
            string op = "PF";
            string sDesc = strings.PF;
            SetOneVal(op, sDesc, positionID);
        }

        public void DetectLiquid(int subMergeDistance = 0, int neededDistance = 0)
        {
            string op = "ZX";
            string sDesc = strings.ZX;
            List<int> vals = new List<int>() { subMergeDistance, neededDistance };
            SendCommand(op, sDesc, vals);
        }
        #endregion
        #region Tip
        public void GetTip(int x, int y, int zStart, int zEnd, int minSteps = 10, int maxSteps = 60)
        {
            string op = "FT";
            string sDesc = strings.FT;
            List<int> vals = new List<int>() {x,y,zStart,zEnd,minSteps,maxSteps };
            SendCommand(op, sDesc, vals);
        }

        public void DropTip(int x, int y, int z, int steps = 60)
        {
            string op = "BT";
            string sDesc = strings.BT;
            List<int> vals = new List<int>() {x,y,z,steps};
            SendCommand(op, sDesc, vals);
        }

        #endregion

      
        //#region report
        //int GetAxisPosition(string XYZ)
        //{
        //    string op = "RG";
        //    string sDesc = string.Format(strings.RG, XYZ);
        //    int val = 0;
        //    switch (XYZ)
        //    {
        //        case "Y":
        //            val = 1;
        //            break;
        //        case "Z":
        //            val = 2;
        //            break;
        //    }
        //    SetOneVal(op, sDesc, val);
        //}

        //static public int CurX { get; set; }

        //#endregion
        
    }





    

    public class Layout
    {
        

    }
}
