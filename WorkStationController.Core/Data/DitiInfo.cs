using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Core.Data
{
    [Serializable]
    public class DitiInfo
    {
        
        public List<DitiInfoItem> DitiInfoItems { get; set; }
        public string CurrentDitiLabware { get; set; }

        public DitiInfo()
        {
            DitiInfoItems = new List<DitiInfoItem>();
            CurrentDitiLabware = "";
        }
        public DitiInfo(string currentDiti,List<DitiInfoItem> ditiInfoItems)
        {
           
            CurrentDitiLabware = currentDiti;
            DitiInfoItems = ditiInfoItems;
        }
    }

    [Serializable]
    public class DitiInfoItem
    {
        public string label;
        public int count;
        public DitiInfoItem()
        {

        }
        public DitiInfoItem(KeyValuePair<string, int> labware_cnt)
        {
            // TODO: Complete member initialization
            label = labware_cnt.Key;
            count = labware_cnt.Value;
        }

        public DitiInfoItem(string newLabel, int newCnt)
        {
            // TODO: Complete member initialization
            label = newLabel;
            count = newCnt;
        }
    }
}
