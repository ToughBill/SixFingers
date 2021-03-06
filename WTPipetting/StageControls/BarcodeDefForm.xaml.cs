﻿using SKHardwareController;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.Hardware;
using WTPipetting.Navigation;
using WTPipetting.Utility;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for BarcodeDefForm.xaml
    /// </summary>
    public partial class BarcodeDefForm :  BaseUserControl
    {
       
        int tubeID = 0;
        Dictionary<int, string> ID_Barcode;
        bool validPosition = false;
        public BarcodeDefForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
            InitDataGridView();
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            this.Loaded += BarcodeDefForm_Loaded;
        }


        protected override void onStageChanged(object sender, EventArgs e)
        {
            base.onStageChanged(sender, e);
            if(GlobalVars.Instance.FarthestStage == Stage.BarcodeDef)
            {
                if (BarcodeScanner.Instance.IsOpen)
                {
                    BarcodeScanner.Instance.onNewBarcode += Instance_onNewBarcode;
                }
            }
        }
        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int grid = e.ColumnIndex + 1;
            if (e.ColumnIndex * 16 + e.RowIndex + 1 > GlobalVars.Instance.SampleCount)
                return;

            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            int tempTubeID = 16 * e.ColumnIndex + e.RowIndex + 1;
            string newBarcode = cell.Value.ToString();
            if(ID_Barcode.ContainsKey(tempTubeID))
                ID_Barcode[tempTubeID] = newBarcode;
        }

        void BarcodeDefForm_Loaded(object sender, RoutedEventArgs e)
        {
            tubeID = 1;
            ID_Barcode = new Dictionary<int, string>();
        }

        void Instance_onNewBarcode(object sender, string e)
        {
            this.Dispatcher.Invoke(()=>
            {
                UpdateDatagridView(e);
            });
            
        }

        private void SetInfo(string s,bool isError)
        {
            txtInfo.Text = s;
            txtInfo.Foreground = isError ? Brushes.Red : Brushes.Black;
        }

        

        private void UpdateDatagridView(string s)
        {
            //A001=>P001 is position
            if(IsPosition(s))
            {
                validPosition = true;
                int positionID = ParsePositionID(s);
                if (tubeID != positionID)
                    tubeID = positionID;

            }
            else //assign barcode to current position
            {
                if(!validPosition)
                {
                    SetInfo(string.Format("找不到位置信息，忽略条码：{0}", s), true);
                    return;
                }
                if (ID_Barcode.ContainsKey(tubeID))
                    ID_Barcode[tubeID] = s;
                UpdateGridCell(tubeID, s);
                tubeID++;
                validPosition = false;
            }
        }


        private void InitDataGridView()
        {
            dataGridView.AllowUserToAddRows = false;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.Columns.Clear();
            List<string> strs = new List<string>();
            int totalSampleCnt = GlobalVars.Instance.SampleCount;
            int gridCnt = (totalSampleCnt + 15) / 16;

            int srcStartGrid = 1;
            for (int i = 0; i < gridCnt; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = string.Format("条{0}", srcStartGrid + i);
                column.HeaderCell.Style.BackColor = System.Drawing.Color.LightSeaGreen;
                dataGridView.Columns.Add(column);
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;

            }

            dataGridView.RowHeadersWidth = 80;
            for (int i = 0; i < 16; i++)
            {
                dataGridView.Rows.Add(strs.ToArray());
                dataGridView.Rows[i].HeaderCell.Value = string.Format("行{0}", i + 1);
            }
        }


        private void UpdateGridCell(int tubeID, string barcode)
        {
            int gridID = (tubeID - 1) /16 +1;
            int neededGridCnt = (GlobalVars.Instance.SampleCount  + 15)/16;
            if (gridID > neededGridCnt)
                return;

            int rowIndex = (tubeID - 1) % 16;
            dataGridView.Rows[rowIndex].Cells[gridID - 1].Value = barcode;
        }

        private int ParsePositionID(string s)
        {
            char ch = s.First();
            int columnIndex = int.Parse(s.Substring(1));
            return columnIndex * 16 + (ch - 'A') + 1;
        }

        private bool IsPosition(string s)
        {
            char ch = s.First();
            int distance = ch - 'A';
            bool isA2P = distance < 16 && distance >= 0; //a,b,c=>p
            int val = 0;
            bool isInt = int.TryParse(s.Substring(1), out val);
            return isA2P && isInt;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Barcode confirmed");
            if(!BarcodeScanner.Instance.IsOpen) //auto fill barcode
            {
                AutoGenerate();
            }
            CheckAllBarcodeDefined();
            GlobalVars.Instance.Tube_Barcode = ID_Barcode;
            NotifyFinished();
        }

        private void AutoGenerate()
        {
            string sData = DateTime.Now.ToString("yyMMddHHmm");
            dataGridView.EndEdit();
            
            for (int i = 0; i < GlobalVars.Instance.SampleCount; i++ )
            {
                
                int col = i / 16;
                int row = i - col * 16;
                string sBarcode = string.Format("{0}_{1}", sData, i+1);
                ID_Barcode.Add(i + 1, sBarcode);
                if (dataGridView[col, row].Value == null || dataGridView[col, row].Value.ToString() == string.Empty)
                    dataGridView[col, row].Value = sBarcode;
            }
              
        }

        private void CheckAllBarcodeDefined()
        {
            
            Dictionary<string, string> barcode_PositionDesc = new Dictionary<string, string>();
            for (int r = 0; r < dataGridView.Rows.Count; r++)
            {
                for (int c = 0; c < dataGridView.Rows[r].Cells.Count; c++)
                {
                    if (c * 16 + r + 1 > GlobalVars.Instance.SampleCount)
                        continue;
                    var cellVal = dataGridView[c, r].Value;
                    string barcode = cellVal == null ? "" : cellVal.ToString();
                    string posDesc = string.Format("条{0}行{1}", c + 1, r + 1);
                    if(barcode == "")
                    {
                        throw new Exception(string.Format("位于{0}的条码为空。",posDesc));
                    }
                    //if(barcode_PositionDesc.ContainsKey(barcode))
                    //    throw new Exception(string.Format("位于{0}与位于{1}的条码重复。", posDesc));
                    barcode_PositionDesc.Add(barcode, posDesc);

                }
            }
        }
    }
}
