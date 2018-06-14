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

        public LabwareTrait GetCurrentDitiBox(DitiType ditiType)
        {
            var ditiInfo = _layout.DitiInfo;
            var labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.GetCurrentLabel(ditiType)); //.CurrentDitiLabwares[ditiType]);
            return labware;
        }

        //public void SetCurrentDitiBox(LabwareTrait labwareTrait)
        //{
        //    var ditiInfo = _layout.DitiInfo;
        //    if (!_layout.LabwareTraits.Contains(labwareTrait))
        //        throw new Exception(string.Format(strings.DitiCannotFind, labwareTrait.Label));
        //    DitiType ditiType = DitiBox.Parse(labwareTrait.TypeName);
        //    ditiInfo.ChangeCurrentLabel(ditiType, labwareTrait.Label);
        //}


 
        public int GetCurrentDitiID(DitiType ditiType)
        {
            
                var ditiInfo = _layout.DitiInfo;
                DitiBoxInfo ditiInfoItem = ditiInfo.DitiBoxInfos.Find(x => x.label == ditiInfo.GetCurrentLabel(ditiType));
                return ditiInfoItem.count;
            
        }

        
        public Dictionary<LabwareTrait,List<int>> GetTip(DitiType ditiType, int cnt)
        {
            Dictionary<LabwareTrait, List<int>> eachLabware_Tips = new Dictionary<LabwareTrait, List<int>>();

            var ditiInfo = _layout.DitiInfo;
            var thisTypeDitiBoxInfos = ditiInfo.DitiBoxInfos.Where(x => x.type == ditiType).ToList();
            if (thisTypeDitiBoxInfos == null || thisTypeDitiBoxInfos.Count == 0)
                throw new NoDitiBoxException(ditiType);


            string currentLabwareLable = ditiInfo.GetCurrentLabel(ditiType);
            if (!thisTypeDitiBoxInfos.Exists(x => x.label == currentLabwareLable))
                throw new Exception(string.Format(strings.DitiCannotFind, currentLabwareLable));

            DitiBoxInfo ditiBoxInfo = thisTypeDitiBoxInfos.Find(x => x.label == currentLabwareLable);
            var labware = _layout.LabwareTraits.Find(x => x.Label == currentLabwareLable);
            
               
            if (ditiBoxInfo.count >= cnt)
            {
                eachLabware_Tips.Add(labware,GetToUseTipIDs(ditiBoxInfo.count,cnt));
                ditiBoxInfo.count -= cnt;
            }
            else
            {
                eachLabware_Tips.Add(labware, GetToUseTipIDs(ditiBoxInfo.count,ditiBoxInfo.count));
                cnt -= ditiBoxInfo.count;
                
                ditiBoxInfo.count = 0;
                if (thisTypeDitiBoxInfos.Count == 1)
                {
                    throw new NoEngouhDitiException(labware,cnt);
                }
                else //try next one
                {
                    List<DitiBoxInfo> existingDitiLabwares = thisTypeDitiBoxInfos.OrderBy(x => GetID(x)).ToList();
                    int currentIndex = existingDitiLabwares.IndexOf(ditiBoxInfo);
                    currentIndex++;
                    currentIndex = currentIndex % existingDitiLabwares.Count;
                    ditiBoxInfo.isUsing = false;
                    ditiBoxInfo = existingDitiLabwares[currentIndex];
                    ditiBoxInfo.isUsing = true;
                    labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.GetCurrentLabel(ditiType));
                    if (ditiBoxInfo.count > cnt)
                    {
                        eachLabware_Tips.Add(labware,GetToUseTipIDs(ditiBoxInfo.count,cnt));
                        ditiBoxInfo.count -= cnt;
                    }
                    else
                    {
                        eachLabware_Tips.Add(labware, GetToUseTipIDs(ditiBoxInfo.count, ditiBoxInfo.count));
                        cnt -= ditiBoxInfo.count;
                        ditiBoxInfo.count = 0;
                        labware = _layout.LabwareTraits.Find(x => x.Label == ditiInfo.GetCurrentLabel(ditiType));
                        throw new NoEngouhDitiException(labware,cnt);
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

        private int GetID(DitiBoxInfo item)
        {
 	         var labware = _layout.LabwareTraits.Find(x => x.Label == item.label);
            if (labware == null)
                throw new Exception(string.Format(strings.DitiCannotFind, item.label));
            return labware.GridID * 10 + labware.SiteID;
        }



        public void ReplaceTips()
        {
            var ditiInfo = _layout.DitiInfo;
            for(int i = 0; i < ditiInfo.DitiBoxInfos.Count;i++)
            {
                ditiInfo.DitiBoxInfos[i].count = 96;
            }
        }
    }

 

    
}
