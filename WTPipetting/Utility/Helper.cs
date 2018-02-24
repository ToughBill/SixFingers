using WTPipetting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace WTPipetting.Utility
{

    class SerializeHelper
    {
        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
    }
    class DataGridViewHelper
    {
        #region datagrid related
        static public System.Drawing.Color Convert2SystemDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        static public void InitDataGridView(DataGridView dataGridView, Stage stage)
        {
            //dataGridView.Columns.Add(new System.Windows.Forms.DataGridViewColumn());
            dataGridView.Columns.Clear();
            List<string> strs = new List<string>();

            int colNum = (GlobalVars.Instance.SampleCount + 15) / 16;
            for (int j = 0; j < colNum; j++)
                strs.Add("");
            int gridStartPos = GlobalVars.Instance.StartGrid;
            for (int i = 0; i < colNum; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = string.Format("条{0}", gridStartPos + i);
                dataGridView.Columns.Add(column);
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            dataGridView.RowHeadersWidth = 120;
            for (int i = 0; i < 16; i++)
            {
                dataGridView.Rows.Add(strs.ToArray());
                dataGridView.Rows[i].HeaderCell.Value = string.Format("行{0}", i + 1);
            }
        }

        static public void UpdateDataGridView(DataGridView dataGridView, Stage curStage)
        {
            foreach (KeyValuePair<CellPosition, SampleInfo> pair in GlobalVars.Instance.SampleLayoutSettings)
            {
                CellPosition cellPos = pair.Key;
                SampleInfo sampleInfo = pair.Value;
                var cell = dataGridView.Rows[cellPos.rowIndex].Cells[cellPos.colIndex];
                //if (curStage == Stage.BarcodeDef)
                cell.Value = sampleInfo.Barcode;
                //else
                //cell.Value = sampleInfo.ColorfulAssay.Name;
                //cell.Style.BackColor = Convert2SystemDrawingColor(sampleInfo.ColorfulAssay.Color);
            }
        }

        #endregion
    }

    [ValueConversion(typeof(Int32), typeof(Int32))]
    class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = 30;
            try
            {
                width = (double)value;
            }
            catch (Exception)
            {

            }
            width = Math.Max(width, 120);
            width -= 80;
            return width / 6;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            SolidColorBrush solidColorBrush = (SolidColorBrush)value;
            return solidColorBrush.Color;
        }
    }
}
