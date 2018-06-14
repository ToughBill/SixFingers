using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WorkstationController.Core.Data
{
 

    [Serializable]
    public class DitiInfo
    {
        
        public List<DitiBoxInfo> DitiBoxInfos { get; set; }


        //[XmlIgnore]
        //public Dictionary<DitiType,string>  CurrentDitiLabwares { get; set; }


     
       
        public DitiInfo()
        {
            DitiBoxInfos = new List<DitiBoxInfo>();

                
        }
        public DitiInfo(List<DitiBoxInfo> ditiInfoItems)
        {
            DitiBoxInfos = ditiInfoItems;
        }

        internal string GetCurrentLabel(DitiType ditiType)
        {
            var info = DitiBoxInfos.Find(x => IsUsing(x, ditiType));
            if(info == null)
            {
                return "";
            }
            else
            {
                return info.label;
            }
        }

        private bool IsUsing(DitiBoxInfo x, DitiType ditiType)
        {
            return x.type == ditiType && x.isUsing;
        }


        public void SetUsingDitiBox(DitiType ditiType, string label)
        {
            var thisTypeDitiBoxes = DitiBoxInfos.Where(x => x.type == ditiType).ToList();
            for(int i = 0; i< thisTypeDitiBoxes.Count; i++)
            {
                thisTypeDitiBoxes[i].isUsing = thisTypeDitiBoxes[i].label == label;
            }
        }
    }

    [Serializable]
    public class DitiBoxInfo
    {
        public string label;
        public int count;
        public DitiType type;
        public bool isUsing;
        public DitiBoxInfo()
        {

        }
        public DitiBoxInfo(KeyValuePair<string, int> labware_cnt)
        {
            // TODO: Complete member initialization
            label = labware_cnt.Key;
            count = labware_cnt.Value;
            isUsing = false;
             
        }

        public DitiBoxInfo(DitiType ditiType, string newLabel, int newCnt)
        {
            // TODO: Complete member initialization
            label = newLabel;
            count = newCnt;
            this.type = ditiType;
            isUsing = false;
        }
    }
}
