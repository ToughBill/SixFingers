using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core;

namespace WorkstationController.Core.Data
{
    [Serializable]
    public   class PlateVector:BindableBase
    {
        ObservableCollection<ROMAPosition> positions = new ObservableCollection<ROMAPosition>();
        ROMAPosition currentPosition = new ROMAPosition();
        public PlateVector()
        {

        }

        public PlateVector(string name)
        {
            this.name = name;
            ROMAPosition safePosition = new ROMAPosition("Safe", 100, 100, 100, 0);
            ROMAPosition endPosition = new ROMAPosition("End", 100, 100, 100, 0);
            positions.Add(safePosition);
            positions.Add(endPosition);
        }


        public ROMAPosition CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                SetProperty(ref currentPosition, value); 
            }
        }
        


        public ObservableCollection<ROMAPosition> Positions
        {
            get
            {
                return positions;
            }
            set
            {
                SetProperty(ref positions, value);
            }
        }

       

        float speedRatio;
        public float SpeedRatio
        {
            get
            {
                return speedRatio;
            }
            set
            {
                SetProperty(ref speedRatio, value);
            }
        }

        int gripDistance;
        int releaseDistance;
        public int GridDistance
        {
            get
            {
                return gripDistance;
            }
            set
            {
                SetProperty(ref gripDistance, value);
            }
        }

        public int ReleaseDistance
        {
            get
            {
                return releaseDistance;
            }
            set
            {
                SetProperty(ref releaseDistance, value);
            }
        }

        int gripSpeed;
        public int GripSpeed
        {
            get
            {
                return gripSpeed;
            }
            set
            {
                SetProperty(ref gripSpeed, value);
            }
        }

        private string name;

        //public override string ToString()
        //{
        //    return name;
        //}
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetProperty(ref name, value);
            }
        }


        public void RemoveCurrent()
        {

            if (currentPosition != null)
            {
                if (currentPosition.ID == "Safe" || currentPosition.ID == "End")
                    return;
                positions.Remove(currentPosition);
                if (positions.Count != 0)
                    CurrentPosition = positions[0];
            }
                
            
        }

        public void AddNewPosition()
        {
            var IDs = positions.Select(x=>x.ID).ToList();
            List<int> vals = new List<int>();
            foreach(var ID in IDs)
            {
                if( ID == "Safe" || ID == "End")
                    continue;
                vals.Add(int.Parse(ID));
            }

            int nextID = vals.Count > 0 ? vals.Max() +1 : 1;
            positions.Add(new ROMAPosition(nextID.ToString(), 0, 0, 0,0));
        }
    }
}
