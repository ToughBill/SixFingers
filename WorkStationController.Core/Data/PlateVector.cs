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

        int clipDistance;
        int openDistance;
        public int ClipDistance
        {
            get
            {
                return clipDistance;
            }
            set
            {
                SetProperty(ref clipDistance, value);
            }
        }

        public int OpenDistance
        {
            get
            {
                return openDistance;
            }
            set
            {
                SetProperty(ref openDistance, value);
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
            var newPosition = new ROMAPosition(nextID.ToString(), 0, 0, 10, 0);
            //positions.Add(newPosition);
            Positions.Add(newPosition);
            CurrentPosition = newPosition;
        }
    }
}
