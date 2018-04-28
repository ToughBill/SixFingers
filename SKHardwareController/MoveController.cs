using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKHardwareController
{
    public class MoveController : IDisposable
    {
        #region
        protected long received_count = 0;//接收计数  
        protected long send_count = 0;//发送计数  
        protected static byte[] g_ARMSeqNum = new byte[] { 0, 0, };
        protected byte MAX_SEQ_NUM = 7;
        protected byte MASK_SEQ = 0x07;
        protected byte ARM_UART_TIMEOUT = 100; //100ms
        protected byte ARM_UART_TRYCNT = 3;
        protected byte MASK_IVA_BIT = 0x20;
        protected byte MASK_DONE_BIT = 0x10;
        protected byte MASK_REP_BIT = 0x08;
        protected byte ARM_PROTOCOL_STX = 0x02;
        protected byte ARM_PROTOCOL_ETX = 0x03;
        protected byte[] globalBuffer = new byte[1024];
        #endregion

        private SerialPort serialPort = new SerialPort();
        public bool Listening { get; set; }
        bool isErrorState = false;
        bool closing = false;
        bool initialized = false;
        Object thisLock = new Object();
        AutoResetEvent cmdFinished = new AutoResetEvent(false);
        AutoResetEvent cmdACK = new AutoResetEvent(false);
        bool bACK = false;
        double mmPerStepX = 0.22345;
        double mmPerStepY = 0.14224;
        double mmPerStepZ = 0.098175;
        e_RSPErrorCode[] _errorCode = new e_RSPErrorCode[(int)_eARM.两个 - 1];

        private static MoveController instance;
        static public MoveController Instance
        {
            get
            {
                 if(instance == null)
                     instance = new MoveController();
                 return instance;
            }
        }

        /// <summary>
        ///Init ARM
        /// </summary>
        public void Init(string sPort)
        {
            lock (thisLock)
            {
                if (initialized)
                    return;

                try
                {
                    serialPort = new SerialPort(sPort, 9600, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                serialPort.DataReceived += serialPort_DataReceived;
                initialized = true;
            }
        }

        /// <summary>
        /// 启动臂
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveHome(_eARM armid, int timeout)
        {
            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8PI", (int)armid, i==0?false:true));
                cmdACK.WaitOne(100);
                if (bACK) break;
            }

            if (bACK == false)
        {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                if (cmdFinished.WaitOne(timeout) == false) return e_RSPErrorCode.超时错误;
        }
            return _errorCode[(int)armid-1];
        }

        /// <summary>
        /// 移动臂到目标位置
        /// </summary>
        /// <param name="armid">臂ID, 1:左臂，2：右臂</param>
        /// <param name="tbX">x轴位置，单位:mm</param>
        /// <param name="tbY">y轴位置，单位:mm</param>
        /// <param name="tbZ">z轴位置，单位:mm</param>
        /// <param name="timeout">超时等待时间ms, 0：不等待，异步运行</param>
        /// <returns></returns>
        public e_RSPErrorCode MoveXYZ(_eARM armid, double tbX, double tbY, double tbZ, int timeout)
        {
            Stopwatch stopWatcher = new Stopwatch();
            stopWatcher.Start();
            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8PA {1} {2} {3}", (int)armid, 
                    ((int)(tbX / mmPerStepX)).ToString(),
                    ((int)(tbY / mmPerStepY)).ToString(), 
                    ((int)(tbZ * mmPerStepX)).ToString()), i != 0);
                cmdACK.WaitOne(100);
                
                if (bACK) 
                    break;
            }

            if (!bACK)
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                if (!cmdFinished.WaitOne(timeout)) 
                    return e_RSPErrorCode.超时错误;
            }
            Debug.WriteLine("used seconds:" + stopWatcher.Elapsed.Milliseconds.ToString());

            return _errorCode[(int)armid - 1];
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (closing)
                return;
            Listening = true;

            int n = 0;
            byte data = 0;
            byte[] RxBuffer = { 0 };

            do
            {
                n = serialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
                data = (byte)serialPort.ReadByte();//读取缓冲数据  
                if (data == ARM_PROTOCOL_STX)
                    received_count = 0;

                globalBuffer[received_count++] = data;
                if (received_count > 2 && (globalBuffer[received_count - 2] == ARM_PROTOCOL_ETX))
                    break;

            } while (true);


            RxBuffer = new byte[received_count];

            Array.Copy(globalBuffer, RxBuffer, RxBuffer.Length);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Clear();

            bool cmdDone = (RxBuffer[1] & MASK_DONE_BIT) == MASK_DONE_BIT;
            if (cmdDone)
            {
                //log.Info("action done!");
                _errorCode[RxBuffer[2] - 0x31] = (e_RSPErrorCode)(RxBuffer[1] & 0x0f);
                cmdFinished.Set();
                SendACK(RxBuffer[2]);
            }
            else
            {
                bool hasError = (RxBuffer[1] - 0x40) > 0;
                if (hasError)
                {
                    e_RSPErrorCode errorCode = (e_RSPErrorCode)(RxBuffer[1] - 0x40);
                    _errorCode[RxBuffer[2] - 0x31] = (e_RSPErrorCode)(RxBuffer[1] - 0x40);
                    string errDesc = errorCode.ToString();
                    if (IsCriticalError(errorCode))
                    {
                        isErrorState = true;
                        //throw new CriticalException(errDesc);
                    }
                    //log.Error(errDesc);
                    cmdFinished.Set();
                }
                else
                {
                    bACK = true;
                    cmdACK.Set();
                }

            }

            strBuilder.Append("R:");
            //foreach (byte b in buf)
            for (int i = 0; i < received_count; i++)
            {
                strBuilder.Append(RxBuffer[i].ToString("X2") + " ");
            }
            //log.Info(strBuilder.ToString());
            Listening = false;
        }

        private bool IsCriticalError(e_RSPErrorCode errorCode)
        {

            List<e_RSPErrorCode> criticalErrors = new List<e_RSPErrorCode>(){e_RSPErrorCode.未初始化,e_RSPErrorCode.初始化错误,
                e_RSPErrorCode.X失步,e_RSPErrorCode.Y失步,e_RSPErrorCode.Z失步,e_RSPErrorCode.撞臂保护,
                e_RSPErrorCode.RSP_ERROR_Tip_crash,e_RSPErrorCode.RSP_ERROR_Step_loss_X_opp
            };
            return criticalErrors.Contains(errorCode);
        }

        /// <summary>
        /// 发送ACK
        /// </summary>
        /// <param name="armid"></param>
        protected void SendACK(Byte armid)
        {
            byte[] ucData = new byte[6];
            ucData[0] = ARM_PROTOCOL_STX;
            ucData[1] = (byte)0x40;
            ucData[2] = armid;
            ucData[3] = 0x38;
            ucData[4] = ARM_PROTOCOL_ETX;
            ucData[5] = ucData[0];
            for (int i = 1; i < 5; i++)
                ucData[5] = (byte)(ucData[5] ^ ucData[i]);
            serialPort.Write(ucData, 0, ucData.Length);
        }

        protected void SendCommand(string strcmd, bool bRepeat = false)
        {
            var remainContents = serialPort.ReadExisting(); ;
            if (isErrorState)
            {
                throw new Exception("处于错误状态，需要重新初始化！");
            }

            _eARM armid;

            //FUNC_ENTER;
            byte[] data = Encoding.ASCII.GetBytes(strcmd);//获得缓存
            armid = (_eARM)(data[0] - 0x31);

            if (armid >= _eARM.两个)
            {
                //log.ErrorFormat("Wrong ARMID:%d", armid);
                return;
            }

            if (!bRepeat)
            {
                if (g_ARMSeqNum[(int)armid] < MAX_SEQ_NUM)
                    g_ARMSeqNum[(int)armid]++;
                else
                    g_ARMSeqNum[(int)armid] = 1;
            }

            byte[] ucData = new byte[data.Length + 4];
            ucData[0] = ARM_PROTOCOL_STX;
            if (bRepeat)
                ucData[1] = (byte)(0x48 | (g_ARMSeqNum[(int)armid] & MASK_SEQ));
            else
                ucData[1] = (byte)(0x40 | (g_ARMSeqNum[(int)armid] & MASK_SEQ));
            for (int i = 0; i < data.Length; i++)
            {
                ucData[2 + i] = data[i];
            }
            ucData[data.Length + 2] = ARM_PROTOCOL_ETX;
            ucData[data.Length + 3] = ucData[0];
            for (int i = 1; i < data.Length + 3; i++)
                ucData[data.Length + 3] = (byte)(ucData[data.Length + 3] ^ ucData[i]);
            bACK = false;
            _errorCode[(int)armid] = e_RSPErrorCode.RSP_ERROR_NONE;
            serialPort.Write(ucData, 0, ucData.Length);
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }


        public void Dispose()
        {
            Close();
        }

        void Close()
        {
            closing = true;
            if (serialPort != null && serialPort.IsOpen)
            {
                while (Listening)
                    System.Windows.Forms.Application.DoEvents();
                serialPort.Close();
                //log.Info("Port closed.");
            }
        }


    }
    public enum _eARM
    {
        左臂 = 1,
        右臂,
        两个,
    }
    public enum e_RSPErrorCode
    {
        RSP_ERROR_NONE = 0,
        初始化错误,//RSP_ERROR_Initialization_error = 1,
        无效命令,//RSP_ERROR_Invalid_command,
        无效操作,//RSP_ERROR_Invalid_operand,
        无效命令序号,//RSP_ERROR_Invalid_command_sequence, //Invalid command sequence
        动作未完成,//RSP_ERROR_Not_implemented,  //Device not implemented
        超时错误,//RSP_ERROR_Timeout_error,   //Time out error
        未初始化,//RSP_ERROR_Not_initialized,  //Device not initialized
        命令缓存溢出,//RSP_ERROR_Command_overflow, //Command overflow
        没液体ZX,//RSP_ERROR_NoLiquid_ZX,         //No liquid detected with ZX-command
        超范围,//RSP_ERROR_ZMove_out_of_range,   //Entered move for Z-axis out of range
        液体不足ZX,//RSP_ERROR_Not_enough_liquid_ZX,    //Not enough liquid detected with ZX-command
        没液体ZZ,//RSP_ERROR_Noliquid_ZZ,  //No liquid detected with ZZ-command
        液体不足ZZ,//RSP_ERROR_Not_enough_liquid_ZZ, //Not enough liquid detected with ZZ-command
        RSP_ERROR_Reserved1,
        RSP_ERROR_Reserved2,
        RSP_ERROR_Reserved3,
        撞臂保护,//RSP_ERROR_Collision_avoided,//Arm collision avoided
        RSP_ERROR_Reserved4,
        RSP_ERROR_Reserved5,
        X失步,//RSP_ERROR_Step_loss_X,  //Step loss detected on X-axis
        Y失步,//RSP_ERROR_Step_loss_Y,  //Step loss detected on Y-axis
        Z失步,//RSP_ERROR_Step_loss_Z,  //Step loss detected on Z-axis
        RSP_ERROR_Step_loss_X_opp,  //Step loss detected on X-axis of opposing arm
        RSP_ERROR_ALIDUM_pulse_timeout,  //ALIDUM pulse time out
        RSP_ERROR_Tip_not_fetched,  //Tip not fetched (used with DiTi option)
        RSP_ERROR_Tip_crash,    //Tip crash (used with DiTi option)
        RSP_ERROR_Tip_not_clean,//Tip not clean (used with DiTi option)
        Send_fail,
    };
}
