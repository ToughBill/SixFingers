using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using WorkstationController.Core;
using WorkstationController.Core.Data;

namespace WTPipetting.Data
{
    public  class Protocol
    {
        const char comma = ',';
        public string Name { get; set; }

        
        public List<StepDefinition> StepsDefinition { get; set; }
        public Dictionary<int, int> SampleCnt_SecondsNeed { get; set; }
        public Protocol(string name, List<StepDefinition> definitions,Dictionary<int,int> sample_Seconds)
        {
            Name = name;
            StepsDefinition = definitions;
            SampleCnt_SecondsNeed = sample_Seconds;
        }
        public Protocol()
        {

        }
        static public Protocol  CreateFromCSVFile(string csvFile)
        {
            int commaCnt = 4;
            FileInfo fileInfo = new FileInfo(csvFile);
            string name = fileInfo.Name;
            string[] strLines = File.ReadAllLines(csvFile,Encoding.Default);
            strLines = strLines.Where(x => x != "").ToArray();
            List<StepDefinition> stepDefinitions = new List<StepDefinition>();
            

            for (int i = 1; i < strLines.Length; i++ )
            {
                string sLine = strLines[i];
                int currentCnt = sLine.Count(x => x == comma);
                if (currentCnt != commaCnt)
                {
                    throw new Exception(string.Format("CSV文件格式非法,期望列数为{0},实际列数为{1}！",commaCnt,currentCnt));
                }
                string[] lineContents = sLine.Split(',');
                StepDefinition stepDefinition = new StepDefinition(lineContents,i);
                stepDefinitions.Add(stepDefinition);
            }
            Dictionary<int,int> sample_Seconds = ReadTimeInfo(csvFile.Replace(".csv",".txt"));

            return new Protocol(name, stepDefinitions, sample_Seconds);
        }

        private static Dictionary<int, int> ReadTimeInfo(string file)
        {
            
            Dictionary<int, int> pairs = new Dictionary<int, int>();
            if(File.Exists(file))
            {
                List<string> strs = File.ReadAllLines(file).ToList();
                foreach(var str in strs)
                {
                    string[] tempStrs = str.Split(',');
                    pairs.Add(int.Parse(tempStrs[0]), int.Parse(tempStrs[1]));
                }
            }
            return pairs;
        }

       

        
    }

    public enum StepDefCol
    {
        Description = 0,
        SourceLabware = 1,
        Volume = 2,
        DestLabware = 3,
        LiquidClass = 4,
        DitiType = 5
        //TipType = 4,
        //DeadVolume = 5,
        //AspiratePosition = 8,
        //DispensePosition = 9,
        //PreAction = 10,
        //PostAction = 11,
        //DelaySeconds = 12
    }

    public class StepDefinition :BindableBase
    {
        public string Description { get; set; }
        public string SourceLabware { get; set; }
        public string DestLabware { get; set; }
        public int Volume { get; set; }
        //public string RepeatTimes { get; set; }
        //public string TipType { get; set; }
        //public string ReuseTimes { get; set; }
        //public string AspirateConstrain { get; set; }
        //public string DispenseConstrain { get; set; }
        public string LiquidClass { get; set; }
        public int LineNumber { get; set; }
        public DitiType DitiType {get;set;}
        //public int DeadVolume { get; set; }
        //public string PreAction { get; set; }
        //public string PostAction { get; set; }
        //public string DelaySeconds { get; set; }

        public StepDefinition()
        {

        }

        public StepDefinition(string[] lines,int no)
        {
            LineNumber = no;
            Volume = 0;
            //DeadVolume = 0;
            Description = lines[(int)StepDefCol.Description];
            //RepeatTimes = lines[(int)StepDefCol.RepeatTimes];
            string sVolume = lines[(int)StepDefCol.Volume];
            if (sVolume == "")
                return;
            Volume = int.Parse(sVolume);
            //string sDeadVolume = lines[(int)StepDefCol.DeadVolume];
            //DeadVolume = int.Parse(sDeadVolume);
            SourceLabware = lines[(int)StepDefCol.SourceLabware];
            DestLabware = lines[(int)StepDefCol.DestLabware];
            //TipType = lines[(int)StepDefCol.TipType];
            LiquidClass = lines[(int)StepDefCol.LiquidClass];
            DitiType =(DitiType) Enum.Parse(typeof(DitiType),lines[(int)StepDefCol.DitiType]);
        }

        public const string MovePlate = "MovePlate";
    }

    class StepDefinitionWithProgressInfo : StepDefinition
    {
        private bool _isWorking = false;
        private bool _isFinished = false;

        public bool IsWorking 
        {
            get
            {
                return _isWorking;
            }
            set
            {
                SetProperty(ref _isWorking, value);
            }
        }
        public bool IsFinished 
        {
            get
            {
                return _isFinished;
            }
            set
            {
                SetProperty(ref _isFinished, value);
            }
        }
        public StepDefinitionWithProgressInfo(StepDefinition stepDef)
        {
            Description = stepDef.Description;
            SourceLabware = stepDef.SourceLabware;
            Volume = stepDef.Volume;
            //DeadVolume = stepDef.DeadVolume;
            DestLabware = stepDef.DestLabware;
            //RepeatTimes = stepDef.RepeatTimes;
            //ReuseTimes = stepDef.ReuseTimes;
            //TipType = stepDef.TipType;
            LineNumber = stepDef.LineNumber;
        }

    }
}
