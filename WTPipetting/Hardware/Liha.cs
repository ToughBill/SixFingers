using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using MotorDriver;
using System.Windows.Forms;

namespace WTPipetting.Hardware
{
    class Liha : IHardwareDevice
    {
        Pump pump;
        Layout layout;
        TipManagement tipManagement;
        //RSPDriver motoDriver = new RSPDriver();
        DebugDriver motoDriver = new DebugDriver();
        float _currentx = 0;
        float _currenty = 0;
        float _currentz = 0;
        public void Init()
        {
            pump = new Pump();
            //pump.Init();
            tipManagement = new TipManagement(layout);
            motoDriver.InitLeft();

            _currentx = 0;
            _currenty = 0;
            _currentz = 0;
        }

        public Liha(Layout layout)
        {
            this.layout = layout;
        }

        public void Move2AbsolutePosition(float x, float y, float z)
        {
            //throw new NotImplementedException();

            _currentx = x;
            _currenty = y;
            _currentz = z;
            motoDriver.MoveLeftAbsolute(_currentx, _currenty, _currentz);
        }

        public void MoveXAbs(float x)
        {
            _currentx = x;
            motoDriver.MoveLeftAbsolute(_currentx, _currenty, _currentz);
        }

        public void MoveYAbs(float y)
        {
            _currenty = y;
            motoDriver.MoveLeftAbsolute(_currentx, _currenty, _currentz);
        }

        public void MoveZAbs(float z)
        {
            _currentz = z;
            motoDriver.MoveLeftAbsolute(_currentx, _currenty, _currentz);
        }

        public void MoveRelativeX(float x, float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveRelativeY(float y, float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveRelativeZ(float z, float speed)
        {
            throw new NotImplementedException();
        }

        public void GetTip()
        {
            var labware_TipIDs = tipManagement.GetTip(1).First();
            var labware = layout.FindLabware(labware_TipIDs.Key.Label);
            if (labware == null)
                throw new NoLabwareException(labware.Label);
            var position = labware.GetPosition(labware_TipIDs.Value.First());
            Move2AbsolutePosition(position);
            //move the diti cone down and search for tip
        }

        public void DropTip()
        {
            var labware = layout.FindLabwareByType("Waste"); 
            var position = labware.GetPosition(1);
            Move2AbsolutePosition(position);
            //drop the tip
        }


        public void Aspirate(double volume, string liquidClass)
        {
            //throw new NotImplementedException();
            //MessageBox.Show("Aspirate " + volume + " " + liquidClass);
        }

        public void Dispense(double volume, string liquidClass)
        {
            //throw new NotImplementedException();
            //MessageBox.Show("Dispense " + volume + " " + liquidClass);
        }

        internal void Move2AbsolutePosition(System.Windows.Point position)
        {
            //throw new NotImplementedException();
            _currentx = (float)position.X;
            _currenty = (float)position.Y;
            //_currentz = z;
            motoDriver.MoveLeftAbsolute(_currentx, _currenty, _currentz);
        }

        
    }
}
