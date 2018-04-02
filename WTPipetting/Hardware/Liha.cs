using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;

namespace WTPipetting.Hardware
{
    class Liha : IHardwareDevice
    {
        Pump pump;
        Layout layout;
        TipManagement tipManagement;
        public void Init()
        {
            pump = new Pump();
            pump.Init();
            tipManagement = new TipManagement(layout);
            
        }

        public Liha(Layout layout)
        {
            this.layout = layout;
        }

        public void Move2AbsolutePosition(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void MoveXAbs(float x)
        {
            throw new NotImplementedException();
        }

        public void MoveYAbs(float y)
        {
            throw new NotImplementedException();
        }

        public void MoveZAbs(float z)
        {
            throw new NotImplementedException();
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
            var labware = layout.FindLabware("Waste");
            if (labware == null)
                throw new NoLabwareException(labware.Label);
            var position = labware.GetPosition(1);
            Move2AbsolutePosition(position);
            //drop the tip

           
        }


        public void Aspirate(double volume, string liquidClass)
        {
            throw new NotImplementedException();
        }

        public void Dispense(double volume, string liquidClass)
        {
            throw new NotImplementedException();
        }

        internal void Move2AbsolutePosition(System.Windows.Point position)
        {
            throw new NotImplementedException();
        }

        
    }
}
