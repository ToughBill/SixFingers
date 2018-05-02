using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkstationController.Core.Data
{
    public class PlateVectorCollection:BindableBase
    {
        ObservableCollection<PlateVector> plateVectors = new ObservableCollection<PlateVector>();
        PlateVector selectedPlateVector = new PlateVector();
        public PlateVector SelectedPlateVector
        {
            get
            {
                return selectedPlateVector;
            }

            set
            {
                SetProperty(ref selectedPlateVector, value);
            }
        }

        public ObservableCollection<PlateVector> PlateVectors
        {
            get
            {
                return plateVectors;
            }
            set
            {
                SetProperty(ref plateVectors, value);
            }
        }

        public void Add(PlateVector plateVector)
        {
            plateVectors.Add(plateVector);
            SelectedPlateVector = plateVector;
        }
    } 
}
