using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    //pump part
    public partial class RSPCommandsImpl
    {
        Dictionary<byte, Dictionary<byte, int>> pumpID_PlungerPositionOfEachArm = null;

        public void InitializePumps()
        {
            foreach(var pair in armID_Address)
            {
                InitializePump(pair.Key);
            }
        }

        public void InitializePump(byte armID)
        {
            var pumpID_AddressDict = pumpID_AddressOfEachArm[armID];
            const int defaultInputPort = 0;
            const int defaultOutputPort = 0;
            int fullForce = 0;
            int inputPortNum = defaultInputPort;
            int outputPortNum = defaultOutputPort;

            //initialize every pump of the arm
            foreach (var pumpID_Address in pumpID_AddressDict)
            {
                string sCommand = string.Format("Z{0},{1},{2}", fullForce, inputPortNum, outputPortNum);
                SendPumpCommand(sCommand, armID, pumpID_Address.Key);
            }

            //set acceleration speed for each pump
            int defaultSlope = 7;
            foreach (var pumpID_Address in pumpID_AddressDict)
            {
                SetPungerAcceleration(armID, pumpID_Address.Key, defaultSlope);
            }
        }

        public void SetValveInput(byte armID, byte pumpID)
        {
            string sCommand = "I";
            SendPumpCommand(sCommand, armID, pumpID);
        }

        public void SetValveOutput(byte armID, byte pumpID)
        {
            string sCommand = "O";
            SendPumpCommand(sCommand, armID, pumpID);
        }

        private void SetPlugerSpeed(byte armID, byte pumpID, int pulsePerSecond, string speedType)
        {
            string speedAbbr = "";
            switch (speedType)
            {
                case start:
                    speedAbbr = "v";
                    break;
                case top:
                    speedAbbr = "V";
                    break;
                case cutoff:
                    speedAbbr = "c";
                    break;

            }
            if (pulsePerSecond > 1000 || pulsePerSecond < 50)
                throw new ArgumentOutOfRangeException(string.Format("{0} speed must between 50 and 1000!", speedType));
            string sCommand = string.Format("{0}{1}", speedAbbr, pulsePerSecond);
            SendPumpCommand(sCommand);
        }

        public void SetPlungerStartSpeed(byte armID, byte pumpID, int pulsePerSecond)
        {
            SetPlugerSpeed(armID, pumpID, pulsePerSecond, start);
        }

        public void SetPlungerTopSpeed(byte armID, byte pumpID, int pulsePerSecond)
        {
            SetPlugerSpeed(armID, pumpID, pulsePerSecond, top);
        }

        public void SetPlungerCutOffSpeed(byte armID, byte pumpID, int pulsePerSecond)
        {
            SetPlugerSpeed(armID, pumpID, pulsePerSecond, cutoff);
        }

        public void Pipette(byte armID, 
                            byte pumpID, 
                            int volumeUL, 
                            int ulPerSecond, 
                            bool bAspirate)
        {
            const int maxStepsPerSecond = 1000;
            const int minStepsPerSecond = 50;
            PumpInfo pumpInfo = Configurations.Instance.ArmsInfo[armID].PumpsInfo[pumpID];
            int syringeVolumeUL = pumpInfo.SyringeVolumeUL;
            double maxULPerSecond = CalculateSpeed(maxStepsPerSecond,pumpInfo.TotalMotorSteps,pumpInfo.SyringeVolumeUL);
            double minULPerSecond = CalculateSpeed(minStepsPerSecond, pumpInfo.TotalMotorSteps, pumpInfo.SyringeVolumeUL);
            if( ulPerSecond > maxULPerSecond || ulPerSecond < minULPerSecond)
            {
                string pipetteDesc = bAspirate ? "aspirate" : "dispense";
                throw new ArgumentOutOfRangeException(string.Format("{0} must between {1} and {2}!", ulPerSecond, minULPerSecond,maxULPerSecond));
            }
        }

        private int CalculateSpeed(int maxStepsPerSecond, int totalSteps, int syringeVolumeUL)
        {
            return (int)(syringeVolumeUL * maxStepsPerSecond / (double)totalSteps);
        }

        public void SetPungerAcceleration(byte armID, byte pumpID, int slopeCode)
        {
            if (slopeCode < 1 || slopeCode > 20)
            {
                throw new ArgumentOutOfRangeException("acclerating code must between 1 and 20!");
            }
            string sCommand = string.Format("L{0}", slopeCode);
            SendPumpCommand(sCommand, armID, pumpID);
        }
    }
}
