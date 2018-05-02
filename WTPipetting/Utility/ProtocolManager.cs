using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WTPipetting.Data;

namespace WTPipetting.Utility
{
    class ProtocolManager
    {
        static ProtocolManager _instance;
        List<Protocol> protocols = new List<Protocol>();
        Protocol _protocol;
        static public ProtocolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProtocolManager();
                }
                return _instance;
            }
        }
        private ProtocolManager()
        {
            string sFolder = FolderHelperEx.GetProtocolFolder();
            DirectoryInfo dirInfo = new DirectoryInfo(sFolder);
            var fileInfos = dirInfo.EnumerateFiles("*.csv");
            foreach(var fileInfo in fileInfos)
            {
                protocols.Add(Protocol.CreateFromCSVFile(fileInfo.FullName));
            }
        }

        public  Protocol SelectedProtocol
        {
            get
            {
                return _protocol;
            }
            set
            {
                _protocol = value;
            }
        }
        public List<Protocol> Protocols
        {
            get
            {
                return protocols;
            }
        }


        internal void CheckLayoutMatchProtocol(WorkstationController.Core.Data.Layout selectedLayout, Protocol selectedProtocol)
        {
            var labware = selectedLayout.FindLabwareByType(Labware.WasteLabel);
            if (labware == null)
                throw new NoLabwareException("找不到枪头废弃槽！");

            var stepsDefinition = selectedProtocol.StepsDefinition;
            List<string> labwareNames = new List<string>();
            foreach(var carrier in selectedLayout.Carriers)
            {
                labwareNames.AddRange(carrier.Labwares.Select(x => x.Label));
            }
            foreach(var stepDef in stepsDefinition)
            {
                if(!labwareNames.Contains(stepDef.SourceLabware))
                    throw new Exception(string.Format("找不到名称为：{0}的源器件！", stepDef.SourceLabware));
                if (!labwareNames.Contains(stepDef.DestLabware))
                    throw new Exception(string.Format("找不到名称为：{0}的目标器件！", stepDef.SourceLabware));
                
            }
        }

        internal void CheckLiquidExists(System.Collections.ObjectModel.ObservableCollection<WorkstationController.Core.Data.LiquidClass> liquidClasses, Protocol selectedProtocol)
        {
            var stepsDefinition = selectedProtocol.StepsDefinition;
            var liquidClassNames = liquidClasses.Select(x=>x.SaveName).ToList();
            foreach (var stepDef in stepsDefinition)
            {
                if (!liquidClassNames.Contains(stepDef.LiquidClass))
                    throw new Exception(string.Format("找不到名为{0}的液体类型！", liquidClasses));
            }
        }
    }
}
