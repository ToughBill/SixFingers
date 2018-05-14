using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.Managements
{
    public class TipManagement
    {
        private Layout _layout;
        public TipManagement(Layout layout)
        {
            _layout = layout;
        }

        public LabwareTrait CurrentLabware
        {
            get
            {
                var ditiInfo = _layout.DitiInfo;
                var labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.CurrentDitiLabware);
                return labware;
            }
            set
            {
                var ditiInfo = _layout.DitiInfo;
                if (!_layout.LabwareTraits.Contains(value))
                    throw new Exception(string.Format(strings.DitiCannotFind, value.Label));
                ditiInfo.CurrentDitiLabware = value.Label;
            }
        }

        public int CurrentDitiID
        {
            get
            {
                var ditiInfo = _layout.DitiInfo;
                DitiInfoItem ditiInfoItem = ditiInfo.DitiInfoItems.Find(x => x.label == ditiInfo.CurrentDitiLabware);
                return ditiInfoItem.count;
            }
        }

        
        public Dictionary<LabwareTrait,List<int>> GetTip(int cnt)
        {
            Dictionary<LabwareTrait, List<int>> eachLabware_Tips = new Dictionary<LabwareTrait, List<int>>();

            var ditiInfo = _layout.DitiInfo;
            if (ditiInfo.CurrentDitiLabware == null)
                throw new Exception(strings.NoSpecifiedDiti);

            if (ditiInfo.DitiInfoItems.Count == 0)
                throw new Exception(strings.NoDitiAtAll);

            if (!ditiInfo.DitiInfoItems.Exists( x=> x.label == ditiInfo.CurrentDitiLabware))
                throw new Exception(string.Format(strings.DitiCannotFind, ditiInfo.CurrentDitiLabware));
            DitiInfoItem ditiInfoItem = ditiInfo.DitiInfoItems.Find(x=> x.label == ditiInfo.CurrentDitiLabware);
            var labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.CurrentDitiLabware);
            if (ditiInfoItem.count == 0 && ditiInfo.DitiInfoItems.Count == 1)
            {
                throw new NoTipException(labware, cnt);
            }
               
            if (ditiInfoItem.count >= cnt)
            {
                eachLabware_Tips.Add(labware,GetToUseTipIDs(ditiInfoItem.count,cnt));
                ditiInfoItem.count -= cnt;
                
            }
            else
            {
                eachLabware_Tips.Add(labware, GetToUseTipIDs(ditiInfoItem.count,ditiInfoItem.count));
                cnt -= ditiInfoItem.count;
                
                ditiInfoItem.count = 0;
                
                if (ditiInfo.DitiInfoItems.Count == 1)
                {
                    throw new NoTipException(labware,cnt);
                }
                else //try next one
                {
                    List<DitiInfoItem> existingDitiLabwares = ditiInfo.DitiInfoItems.OrderBy(x => GetID(x)).ToList();
                    int currentIndex = existingDitiLabwares.IndexOf(ditiInfoItem);
                    currentIndex++;
                    currentIndex = currentIndex % existingDitiLabwares.Count;
                    ditiInfoItem = existingDitiLabwares[currentIndex];
                    ditiInfo.CurrentDitiLabware = ditiInfoItem.label;
                    labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.CurrentDitiLabware);
                    if (ditiInfoItem.count > cnt)
                    {
                        eachLabware_Tips.Add(labware,GetToUseTipIDs(ditiInfoItem.count,cnt));
                        ditiInfoItem.count -= cnt;
                    }
                    else
                    {
                        eachLabware_Tips.Add(labware, GetToUseTipIDs(ditiInfoItem.count, ditiInfoItem.count));
                        cnt -= ditiInfoItem.count;
                        ditiInfoItem.count = 0;
                        labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.CurrentDitiLabware);
                        throw new NoTipException(labware,cnt);
                    }
                }
            }
            return eachLabware_Tips;
        }

        private List<int> GetToUseTipIDs(int remainCount, int toUseCount)
        {
            List<int> tipIDs = new List<int>();
            if(remainCount < toUseCount)
            {
                throw new Exception("所剩枪头不够！");
            }
            int startID = 96 - remainCount + 1;
            for(int i = 0; i< toUseCount; i++)
            {
                int tipID = startID + i;
                tipIDs.Add(tipID);
            }
            return tipIDs;
        }

        private int GetID(DitiInfoItem item)
        {
 	         var labware = _layout.LabwareTraits.Find(x => x.Label == item.label);
            if (labware == null)
                throw new Exception(string.Format(strings.DitiCannotFind, item.label));
            return labware.GridID * 10 + labware.SiteID;
        }



        public void ReplaceTips()
        {
            var ditiInfo = _layout.DitiInfo;
            for(int i = 0; i < ditiInfo.DitiInfoItems.Count;i++)
            {
                ditiInfo.DitiInfoItems[i].count = 96;
            }
        }
    }

 

    
}
