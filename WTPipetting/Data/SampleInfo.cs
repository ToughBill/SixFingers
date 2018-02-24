using WTPipetting.Properties;
using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WTPipetting.Utility;

namespace WTPipetting.Data
{
    public class SampleLayoutSettings : IEnumerable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        Dictionary<CellPosition, SampleInfo> allSampleLayoutInfos = new Dictionary<CellPosition, SampleInfo>();

        public void SetLine(int lineIndex, List<SampleInfo> cellInfosThisLine, int startIndex = 0)
        {
            for (int i = 0; i < cellInfosThisLine.Count; i++)
            {
                CellPosition cellPos = new CellPosition(lineIndex, i + startIndex);
                allSampleLayoutInfos[cellPos] = cellInfosThisLine[i];
            }
        }

        public void Sort()
        {
            allSampleLayoutInfos = allSampleLayoutInfos.OrderBy(x => x.Key.colIndex * 16 + x.Key.rowIndex).ToDictionary(x => x.Key, x => x.Value);
        }


        public int Count
        {
            get
            {
                return this.allSampleLayoutInfos.Count;
            }
        }

        public ObservableCollection<SampleInfo> SampleInfoList
        {
            get { return new ObservableCollection<SampleInfo>(this.allSampleLayoutInfos.Values); }
        }
        public bool Contains(int r, int c)
        {
            return allSampleLayoutInfos.ContainsKey(new CellPosition(c, r));
        }
        public List<SampleInfo> GetLine(int lineIndex)
        {
            return allSampleLayoutInfos.Where(x => x.Key.colIndex == lineIndex).Select(x => x.Value).ToList();
        }
        public void Clear()
        {
            allSampleLayoutInfos.Clear();
        }

        public SampleInfo this[int wellIndex]
        {
            get
            {
                try
                {
                    return allSampleLayoutInfos[new CellPosition(wellIndex)];
                }
                catch (Exception)
                {
                    // Catch all exceptions and return null
                    return null;
                }
            }
            set
            {
                allSampleLayoutInfos[new CellPosition(wellIndex)] = value;
            }
        }



        public SampleInfo this[int r, int c]
        {
            get
            {
                try
                {
                    return allSampleLayoutInfos[new CellPosition(c, r)];
                }
                catch (Exception)
                {
                    // Catch all exceptions and return null
                    return null;
                }
            }
            set
            {
                allSampleLayoutInfos[new CellPosition(c, r)] = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("SampleInfoList"));
            }
        }

        public SampleInfo this[CellPosition cellPos]
        {
            get
            {
                return allSampleLayoutInfos[cellPos];
            }
            set
            {
                allSampleLayoutInfos[cellPos] = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("SampleInfoList"));
            }
        }

        //public bool AlreadyHasInfo()
        //{
        //    var list = allSampleLayoutInfos.Values.ToList();
        //    return list.Exists(x => IsMeanfulAssay(x.ColorfulAssay.Name));
        //}

        //private bool IsMeanfulAssay(string s)
        //{
        //    return s != "" && s != stringRes.EmptyAssay;
        //}

        public IEnumerator GetEnumerator()
        {
            foreach (var pair in allSampleLayoutInfos)
                yield return pair;
        }



        public bool AllSet(ref string errMsg)
        {
            Sort();
            int nCount = GlobalVars.Instance.SampleCount;
            for (int i = 0; i < nCount; i++)
            {
                CellPosition cellPosition = new CellPosition(i);
                if (GlobalVars.Instance.SampleLayoutSettings[cellPosition] == null || GlobalVars.Instance.SampleLayoutSettings[cellPosition].Barcode == "")
                {
                    
                    errMsg = string.Format("位于{0}处的样品未设置条码！", CellPosition.GetDescription(cellPosition));
                    return false;
                }
            }
            return true;
        }

        internal static SampleLayoutSettings Create(int smpCnt)
        {
            SampleLayoutSettings layoutSettings = new SampleLayoutSettings();
            for (int i = 0; i < smpCnt; i++)
                layoutSettings[i] = new SampleInfo("");
            return layoutSettings;
        }
    }

    public class SampleInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private ColorfulAssay _colorfulAssay;
        private string _barcode;

        //public ColorfulAssay ColorfulAssay
        //{
        //    get
        //    {
        //        return _colorfulAssay;
        //    }
        //    set
        //    {
        //        _colorfulAssay = value;
        //        PropertyChanged(this, new PropertyChangedEventArgs("ColorfulAssay"));
        //    }
        //}

        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                _barcode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Barcode"));
            }
        }

        public SampleInfo( string barcode)
        {
            this.Barcode = barcode;
            //this.ColorfulAssay = colorfulAssay;
        }
    }

    public struct CellPosition
    {
        public int colIndex;
        public int rowIndex;
        public CellPosition(int ix, int iy)
        {
            colIndex = ix;
            rowIndex = iy;
        }

        public CellPosition(int wellIndex)
        {
            colIndex = wellIndex / 16;
            rowIndex = wellIndex - 16 * colIndex;
        }

        static public string GetDescription(CellPosition cellPosition)
        {
            int gridStartPos = GlobalVars.Instance.StartGrid;
            return string.Format("[条{0}行{1}]", gridStartPos + cellPosition.colIndex, cellPosition.rowIndex + 1);
        }
    }
}
