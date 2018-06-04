using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WorkstationController.Hardware
{
    class BarcodeScanner
    {
        private SerialPort scannerPort = new SerialPort();
        Object thisLock = new Object();
        private bool isOpened = false;
        public event EventHandler<string> onNewBarcode;
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <returns></returns>
        public string SannnerGetLabel()
        {
            string ret = scannerPort.ReadLine();
            return ret.Substring(1, ret.Length - 1);//去掉最后一个\r

        }
        void NotifyNewBarcode(string barcode)
        {
            if (onNewBarcode != null)
                onNewBarcode(this, barcode);
        }


        private static BarcodeScanner instance;

        static public BarcodeScanner Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BarcodeScanner();
                }
                return instance;
            }
        }
     

        /// <summary>
        /// 初始化扫描器
        /// </summary>
        /// <param name="sPort"></param>
        public void Open(string sPort)
        {
            lock (thisLock)
            {
                if (isOpened)
                    return;

                try
                {
                    scannerPort = new SerialPort(sPort, 9600, Parity.None, 8, StopBits.One);
                    scannerPort.Open();
                    SannerSetContinueTrigger(true);
                    Thread barcodeThread = new Thread(BarcodeReceiveThread);
                    barcodeThread.Start();

                    isOpened = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        void BarcodeReceiveThread()
        {
            if (!isOpened)
                return;
            while (isOpened)
            {
                if (scannerPort == null)
                    break;
                if (!scannerPort.IsOpen)
                    break;
                if (scannerPort.BytesToRead == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                string s = SannnerGetLabel();
                NotifyNewBarcode(s);
            }
        }

        /// <summary>
        /// 设置扫描器是否连续触发
        /// </summary>
        /// <param name="bOnoff"></param>
        /// <returns></returns>
        public bool SannerSetContinueTrigger(bool bOnoff)
        {
            if (bOnoff)
            {
                string command = string.Format("\x02PT03203401\r\n");
                scannerPort.WriteLine(command);
            }
            else
            {
                string command = string.Format("\x02PT03203400\r\n");
                scannerPort.WriteLine(command);
            }
            string ret = scannerPort.ReadLine();
            ret.Trim();
            if (ret == "\x02PS0\r")
                return true;
            else
                return false;
        }

        /// <summary>
        /// 设置相同标签是否触发
        /// </summary>
        /// <param name="bOnoff"></param>
        /// <returns></returns>
        public bool ScannerSetSameTrigger(bool bOnoff)
        {
            if (bOnoff)
            {
                string command = string.Format("\x02PT00208600\r\n");
                scannerPort.WriteLine(command);
            }
            else
            {
                string command = string.Format("\x02PT00208680\r\n");
                scannerPort.WriteLine(command);
            }
            string ret = scannerPort.ReadLine();
            ret.Trim();
            if (ret == "\x02PS0\r")
                return true;
            else
                return false;

        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public string ScannerGetVersion()
        {
            string command = string.Format("\x02V\r\n");
            scannerPort.WriteLine(command);
            string version = scannerPort.ReadLine();
            version = version.Substring(0, version.Length - 1);//去掉最后一个\r
            version = version.Substring(1);//去掉第一个字\x02
            return version;
        }

        public void Close()
        {
            if (scannerPort != null && scannerPort.IsOpen)
            {
                SannerSetContinueTrigger(false);
                scannerPort.Close();
                //log.Info("Port closed.");
            }
            isOpened = false;
        }
        

        public bool IsOpen
        {
            get
            {
                return isOpened;
            }
        }
    }
}
