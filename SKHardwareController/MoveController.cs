using System;
using System.Collections.Generic;
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

        public const double MAX_X_DISTANCE = 700.0;//70cm
        public const double MAX_Y_DISTANCE = 350.0;//40cm
        public const double MAX_Z_DISTANCE = 250.0;//30cm
        private const double DegreePerStep = 360 / 200;

        private int getValue = 0;

        private SerialPort serialPort = new SerialPort();
        
        public bool Listening { get; set; }
        public bool bOpen = false;
        
        
        public bool bHome = false;
        private bool bClipperInit = false;
        private bool bADPInited = false;
        Object thisLock = new Object();
        //AutoResetEvent cmdFinished = new AutoResetEvent(false);
        AutoResetEvent cmdACK = new AutoResetEvent(false);
        bool[] bACK = new bool[2];
        bool[] bActiondone = new bool[2];
        double mmPerStepX = 89.5/200;
        double mmPerStepY = 56.8/200;
        double mmPerStepZ = 56.8/200;
        double mmPerStepClipper = 0.1;//mm
        double ulPerStepADP = 0.025;//ul/step
        e_RSPErrorCode[] _errorCode = new e_RSPErrorCode[(int)_eARM.两个 - 1];
        public const int defaultTimeOut = 10000;
        private static MoveController instance;
        public event EventHandler<string> onCriticalErrorHappened;
   
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
                if (bOpen)
                    return;

                try
                {
                    serialPort = new SerialPort(sPort, 9600, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                    //serialPort.DataReceived += serialPort_DataReceived;

                    Thread myThread = new Thread(ReceiveMessage);
                    myThread.IsBackground = true;
                    myThread.Start();

                    bOpen = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool ClipperInitialized
        {
            get
            {
                return bClipperInit;
            }
        }


        public bool ADPInitialized
        {
            get
            {
                return bADPInited;
            }
        }


        public bool XYZInitialized
        {
            get
            {
                return bHome;
            }
        }


        /// <summary>
        /// 张开夹爪
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveClipper(double width)
        {
            return MoveClipperAtSpeed(_eARM.右臂, width, 10);
        }

        /// <summary>
        /// 旋转夹爪
        /// </summary>
        /// <param name="Degree"></param>
        /// <returns></returns>
        public e_RSPErrorCode RoateClipper(double Degree, int timeOut = 10000)
        {
            return MoveRAtSpeed(_eARM.右臂, Degree, 180, timeOut);
        }

        public e_RSPErrorCode GetClipperInfo(ref double degree, ref double width)
        {
            degree = GetRPos(_eARM.右臂);
            width = GetClipperPos(_eARM.右臂);
            return e_RSPErrorCode.RSP_ERROR_NONE;
        }


        private bool _istipmounted = false;
        public bool IsTipMounted
        {
            get
            {
                e_RSPErrorCode ret = GetTipStatus(_eARM.左臂, ref _istipmounted);
                return _istipmounted;
            }
        }
        /// <summary>
        /// 初始化ADP
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode InitADP(int timeout = 10000)
        {
            _eARM armid = _eARM.左臂;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AI", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            bADPInited = true;
            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 初始化夹爪
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode InitClipper(int timeout = 10000)
        {
            _eARM armid = _eARM.右臂;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8CI", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }
            bClipperInit = true;
            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 丢弃Tip头
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode DropDiti(int timeout = 10000)
        {
            _eARM armid = _eARM.左臂;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AA", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 液位探测
        /// </summary>
        /// <param name="zStart"></param>
        /// <param name="zMax"></param>
        /// <param name="speedMMPerSecond"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode DetectLiquid(double zStart, double zMax, double speedMMPerSecond, int timeout=10000)
        {
            _eARM armid = _eARM.左臂;
            e_RSPErrorCode ret = e_RSPErrorCode.RSP_ERROR_NONE;

            //移动到z start
            ret = Move2Z(armid, zStart);
            if (ret != e_RSPErrorCode.RSP_ERROR_NONE)
                return ret;

            //开启液位探测功能
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AL", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            //探测直到zMax
            ret = MoveZAtSpeed(armid, zMax - zStart, speedMMPerSecond, 15000);
            return ret;
        }

        /// <summary>
        /// 停止ADP
        /// </summary>
        /// <returns></returns>
        public e_RSPErrorCode StopLiquidDetection()
        {
            _eARM armid = _eARM.左臂;
            int timeout = 1000;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8PS", (int)armid));
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 吸液
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="speedMax">ul/s</param>
        /// <param name="speedStart">ul/s</param>
        /// <param name="speedStop">ul/s</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode Aspirate(double volume, double speedMax=100, double speedStart=50, double speedStop=50, int timeout=10000)
        {
            _eARM armid = _eARM.左臂;
            int bNewFunc = 0;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AP {1} {2} {3} {4} {5}", (int)armid,
                    Math.Round(speedMax / ulPerStepADP), Math.Round(speedStart / ulPerStepADP), Math.Round(speedStop / ulPerStepADP), 
                    (int)volume, bNewFunc), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 吐液
        /// </summary>
        /// <param name="volume">吐液量ul</param>
        /// <param name="speedMax">最大速度ul/s</param>
        /// <param name="speedStart">ul/s</param>
        /// <param name="speedStop">ul/s</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode Dispense(double volume, double speedMax=1000, double speedStart=100, double speedStop=200, int timeout=10000)
        {
            _eARM armid = _eARM.左臂;

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AD {1} {2} {3} {4}", (int)armid,
                    Math.Round(speedMax / ulPerStepADP), Math.Round(speedStart / ulPerStepADP), Math.Round(speedStop / ulPerStepADP), (int)volume), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="axis"></param>
        /// <param name="DegreeOrMMPerSecond">角度或距离</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode StartMove(_eARM armid, Axis axis, double DegreeOrMMPerSecond, double distance, int timeout=20000)
        {
            int speedbyStep = 0;
            int distancebystep = 0;

            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }

            switch (axis)
            {
                case Axis.X:
                    speedbyStep = (int)Math.Round(DegreeOrMMPerSecond / mmPerStepX);
                    distancebystep =(int)Math.Round(distance / mmPerStepX);
                    break;
                case Axis.Y:
                    speedbyStep = (int)Math.Round(DegreeOrMMPerSecond / mmPerStepY);
                    distancebystep = (int)Math.Round(distance / mmPerStepY);
                    break;
                case Axis.Z:
                    speedbyStep = (int)Math.Round(DegreeOrMMPerSecond / mmPerStepZ);
                    distancebystep = (int)Math.Round(distance / mmPerStepZ);
                    break;
                case Axis.R:
                    speedbyStep = (int)Math.Round(DegreeOrMMPerSecond / DegreePerStep);
                    distancebystep = (int)Math.Round(distance / DegreePerStep);
                    break;
                case Axis.Clipper:
                    speedbyStep = (int)Math.Round(DegreeOrMMPerSecond / mmPerStepClipper);
                    distancebystep = (int)Math.Round(distance / mmPerStepClipper);
                    break;
            }


            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8SM {1} {2} {3}", (int)armid, (int)axis, speedbyStep, distancebystep), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                StopMove(armid, axis);
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public e_RSPErrorCode StopMove(_eARM armid, Axis axis, int timeout=500)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8ST {1}",(int)armid, (int)axis), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 设定最大速度
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="axis"></param>
        /// <param name="maxSpeedMMPerSecond"></param>
        /// <returns></returns>
        public e_RSPErrorCode SetSpeed(_eARM armid, e_CanMotorID motor, double maxSpeedMMorDegreePerSecond)
        {
            int timeout = 500;
            double SpeedbyStep = 0;

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作; ;
            }

            switch (motor)
            {
                case e_CanMotorID.CanMotorID_Left_x:
                case e_CanMotorID.CanMotorID_Right_x:
                    SpeedbyStep = Math.Round(maxSpeedMMorDegreePerSecond/mmPerStepX);
                    break;
                case e_CanMotorID.CanMotorID_Left_y:
                case e_CanMotorID.CanMotorID_Right_y:
                    SpeedbyStep = Math.Round(maxSpeedMMorDegreePerSecond/mmPerStepY);
                    break;
                case e_CanMotorID.CanMotorID_Left_z:
                case e_CanMotorID.CanMotorID_Right_z:
                    SpeedbyStep = Math.Round(maxSpeedMMorDegreePerSecond/mmPerStepZ);
                    break;
                case e_CanMotorID.CanMotorID_Rotate:
                    SpeedbyStep = Math.Round(maxSpeedMMorDegreePerSecond/DegreePerStep);
                    break;
                case e_CanMotorID.CanMotorID_Clipper:
                    SpeedbyStep = Math.Round(maxSpeedMMorDegreePerSecond/mmPerStepClipper);
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8SS {1} {2}", (int)armid, (int)motor, (int)SpeedbyStep), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 设定最大加速度
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="axis"></param>
        /// <param name="maxAccSpeedMMPerSecond"></param>
        /// <returns></returns>
        public e_RSPErrorCode SetAccSpeed(_eARM armid, e_CanMotorID motor, double maxAccSpeedMMorDegreePerSecond)
        {
            int timeout = 500;
            double SpeedbyStep = 0;

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作; ;
            }

            switch (motor)
            {
                case e_CanMotorID.CanMotorID_Left_x:
                case e_CanMotorID.CanMotorID_Right_x:
                    SpeedbyStep = Math.Round(maxAccSpeedMMorDegreePerSecond / mmPerStepX);
                    break;
                case e_CanMotorID.CanMotorID_Left_y:
                case e_CanMotorID.CanMotorID_Right_y:
                    SpeedbyStep = Math.Round(maxAccSpeedMMorDegreePerSecond / mmPerStepY);
                    break;
                case e_CanMotorID.CanMotorID_Left_z:
                case e_CanMotorID.CanMotorID_Right_z:
                    SpeedbyStep = Math.Round(maxAccSpeedMMorDegreePerSecond / mmPerStepZ);
                    break;
                case e_CanMotorID.CanMotorID_Rotate:
                    SpeedbyStep = Math.Round(maxAccSpeedMMorDegreePerSecond / DegreePerStep);
                    break;
                case e_CanMotorID.CanMotorID_Clipper:
                    SpeedbyStep = Math.Round(maxAccSpeedMMorDegreePerSecond / mmPerStepClipper);
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8SJ {1} {2}", (int)armid, (int)motor, (int)SpeedbyStep), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 启动臂
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveHome(_eARM armid, int timeout)
        {
            //Move2Z
            if (armid == _eARM.左臂 || armid == _eARM.两个)
            {
                for (int i = 0; i < 3; i++)
                {
                    SendCommand(string.Format("18PI"), i != 0);
                    cmdACK.WaitOne(100);
                    if (bACK[0]) break;
                }

                if (!bACK[0])
                {
                    return e_RSPErrorCode.Send_fail;
                }
            }

            if (armid == _eARM.右臂 || armid == _eARM.两个)
            {
                for (int i = 0; i < 3; i++)
                {
                    SendCommand(string.Format("28PI"), i != 0);
                    cmdACK.WaitOne(100);
                    if (bACK[1]) break;
                }

                if (!bACK[1])
                {
                    return e_RSPErrorCode.Send_fail;
                }
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (armid == _eARM.左臂 || armid == _eARM.两个)
            {
                if (timeout > 0)
                {
                    while (!bActiondone[0] && timeout > 0)
                    {
                        Thread.Sleep(10);
                        timeout -= 10;
                    }
                }
                if (timeout <= 0)
                {
                    return e_RSPErrorCode.超时错误;
                }
            }

            if (armid == _eARM.右臂 || armid == _eARM.两个)
            {
                if (timeout > 0)
                {
                    while (!bActiondone[1] && timeout > 0)
                    {
                        Thread.Sleep(10);
                        timeout -= 10;
                    }
                }
                if (timeout <= 0)
                {
                    return e_RSPErrorCode.超时错误;
                }
            }

            if (armid == _eARM.左臂 || armid == _eARM.两个)
            {
                if (_errorCode[0] != e_RSPErrorCode.RSP_ERROR_NONE) 
                    return _errorCode[0];
            }
            if (armid == _eARM.右臂 || armid == _eARM.两个)
            {
                if (_errorCode[1] != e_RSPErrorCode.RSP_ERROR_NONE) 
                    return _errorCode[1];
            }

            bHome = true;
            return e_RSPErrorCode.RSP_ERROR_NONE;
        }

        /// <summary>
        /// 以设定的速度加速度移动x轴，绝对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="absX">绝对位移mm</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode Move2X(_eARM armid, double absX, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8XA {1}", (int)armid, ((int)Math.Round((absX / mmPerStepX))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }


        /// <summary>
        /// 获取x当前位置
        /// </summary>
        /// <param name="armid"></param>
        /// <returns></returns>
        public double GetXPos(_eARM armid)
        {
            double curpos = 0.0;
            int timeout = 500;

            if (!bHome)
            {
                return curpos;
            }

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RX", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue*mmPerStepX;
            return curpos;
        }
        /// <summary>
        /// 获取y当前位置
        /// </summary>
        /// <param name="armid"></param>
        /// <returns></returns>
        public double GetYPos(_eARM armid)
        {
            double curpos = 0.0;
            int timeout = 500;

            if (!bHome)
            {
                return curpos;
            }

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RY", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * mmPerStepY;
            return curpos;
        }

        /// <summary>
        /// 获取z当前位置
        /// </summary>
        /// <param name="armid"></param>
        /// <returns></returns>
        public double GetZPos(_eARM armid)
        {
            double curpos = 0.0;
            int timeout = 500;

            if (!bHome)
            {
                return curpos;
            }

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RZ", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * mmPerStepZ;
            return curpos;
        }

        /// <summary>
        /// 获取抓手当前旋转角度
        /// </summary>
        /// <param name="armid"></param>
        /// <returns></returns>
        public double GetRPos(_eARM armid)
        {
            double curpos = 0.0;
            int timeout = 500;

            if (!bHome)
            {
                return curpos;
            }

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RR", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * DegreePerStep;
            return curpos;
        }

        /// <summary>
        /// 获取抓手张开宽度
        /// </summary>
        /// <param name="armid"></param>
        /// <returns></returns>
        public double GetClipperPos(_eARM armid)
        {
            double curpos = 0.0;
            int timeout = 500;

            if (!bHome)
            {
                return curpos;
            }

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RC", (int)armid), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * mmPerStepClipper;
            return curpos;
        }

        /// <summary>
        /// 查询TIP状态
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="bStatus">true:tip存在，false:tip不存在</param>
        /// <returns></returns>
        public e_RSPErrorCode GetTipStatus(_eARM armid, ref bool bStatus)
        {
            int QueryNum = 31;//查询TIP状态
            int timeout = 500;

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8AQ {1}", (int)armid, QueryNum), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            bStatus = (getValue!=0);
            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 获取电机轴速度
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="motor"></param>
        /// <returns></returns>
        public double GetSpeed(_eARM armid, e_CanMotorID motor)
        {
            double curpos = 0.0;
            int timeout = 500;
            double MMorDegrePperStep = 0;

            if (armid >= _eARM.两个)
            {
                return curpos;
            }
            
            switch(motor)
            {
                case e_CanMotorID.CanMotorID_Left_x:
                case e_CanMotorID.CanMotorID_Right_x:
                    MMorDegrePperStep = mmPerStepX;
                    break;
                case e_CanMotorID.CanMotorID_Left_y:
                case e_CanMotorID.CanMotorID_Right_y:
                    MMorDegrePperStep = mmPerStepY;
                    break;
                case e_CanMotorID.CanMotorID_Left_z:
                case e_CanMotorID.CanMotorID_Right_z:
                    MMorDegrePperStep = mmPerStepZ;
                    break;
                case e_CanMotorID.CanMotorID_Rotate:
                    MMorDegrePperStep = DegreePerStep;
                    break;
                case e_CanMotorID.CanMotorID_Clipper:
                    MMorDegrePperStep = mmPerStepClipper;
                    break;
            }
            
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RS {1}", (int)armid, (int)motor), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * MMorDegrePperStep;
            return curpos;
        }

        /// <summary>
        /// 获取电机轴速度
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="motor"></param>
        /// <returns></returns>
        public double GetAccSpeed(_eARM armid, e_CanMotorID motor)
        {
            double curpos = 0.0;
            int timeout = 500;
            double MMorDegrePperStep = 0;

            if (armid >= _eARM.两个)
            {
                return curpos;
            }

            switch (motor)
            {
                case e_CanMotorID.CanMotorID_Left_x:
                case e_CanMotorID.CanMotorID_Right_x:
                    MMorDegrePperStep = mmPerStepX;
                    break;
                case e_CanMotorID.CanMotorID_Left_y:
                case e_CanMotorID.CanMotorID_Right_y:
                    MMorDegrePperStep = mmPerStepY;
                    break;
                case e_CanMotorID.CanMotorID_Left_z:
                case e_CanMotorID.CanMotorID_Right_z:
                    MMorDegrePperStep = mmPerStepZ;
                    break;
                case e_CanMotorID.CanMotorID_Rotate:
                    MMorDegrePperStep = DegreePerStep;
                    break;
                case e_CanMotorID.CanMotorID_Clipper:
                    MMorDegrePperStep = mmPerStepClipper;
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RJ {1}", (int)armid, (int)motor), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return curpos;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return curpos;
            }

            curpos = getValue * MMorDegrePperStep;
            return curpos;
        }

        /// <summary>
        /// 以设定的速度加速度移动x轴，相对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="x">相对位移mm</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveX(_eARM armid, double x, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8XR {1}", (int)armid, ((int)Math.Round((x / mmPerStepX))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 以指定速度移动x轴,相对位移
        /// </summary>
        /// <param name="armid">臂</param>
        /// <param name="x">相对位移mm</param>
        /// <param name="speed">速度mm/s</param>
        /// <param name="timeout">超时时间ms</param>
        /// <returns></returns>
        public e_RSPErrorCode MoveXAtSpeed(_eARM armid, double x, double speedMMPerSecond, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8XS {1} {2}",
                    (int)armid, ((int)Math.Round((x / mmPerStepX))).ToString(),
                    ((int)Math.Round((speedMMPerSecond / mmPerStepX))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 以设定的速度加速度移动y轴，绝对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="absY">绝对位移mm</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode Move2Y(_eARM armid, double absY, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8YA {1}", (int)armid, ((int)Math.Round((absY / mmPerStepY))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 以设定的速度加速度移动y轴,相对位移
        /// </summary>
        /// <param name="armid">臂</param>
        /// <param name="y">相对位移mm</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveY(_eARM armid, double y, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8YR {1}", (int)armid, ((int)Math.Round((y / mmPerStepY))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 以指定速度移动y轴，相对位移
        /// </summary>
        /// <param name="armid">臂</param>
        /// <param name="x">相对位移mm</param>
        /// <param name="speed">速度mm/s</param>
        /// <param name="timeout">超时时间ms</param>
        /// <returns></returns>
        public e_RSPErrorCode MoveYAtSpeed(_eARM armid, double y, double speedMMPerSecond, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8YS {1} {2}",
                    (int)armid, ((int)Math.Round((y / mmPerStepY))).ToString(),
                    ((int)Math.Round((speedMMPerSecond / mmPerStepY))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 以设定的速度移动z轴,绝对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="absZ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode Move2Z(_eARM armid, double absZ, int timeout = 10000, bool isInitializingClipper = false)
        {
            if (!isInitializingClipper) //we need to move clipper up before move home
            {
                if (!bHome)
                {
                    return e_RSPErrorCode.未初始化;
                }
            }
            

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8ZA {1}", (int)armid, ((int)Math.Round((absZ / mmPerStepZ))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 以设定的加速度速度移动z轴，相对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="z">相对位移</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveZ(_eARM armid, double z, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8ZR {1}",(int)armid, ((int)(z / mmPerStepZ)).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 以指定速度移动z轴，相对位移
        /// </summary>
        /// <param name="armid">臂</param>
        /// <param name="x">相对位移mm</param>
        /// <param name="speed">速度mm/s</param>
        /// <param name="timeout">超时时间ms</param>
        /// <returns></returns>
        public e_RSPErrorCode MoveZAtSpeed(_eARM armid, double z, double speedMMPerSecond, int timeout = 10000)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8ZS {1} {2}",
                    (int)armid, ((int)Math.Round((z / mmPerStepZ))).ToString(),
                    ((int)Math.Round((speedMMPerSecond / mmPerStepZ))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 移动臂到目标位置，绝对位置
        /// </summary>
        /// <param name="armid">臂ID, 1:左臂，2：右臂</param>
        /// <param name="tbX">x轴位置，单位:mm</param>
        /// <param name="tbY">y轴位置，单位:mm</param>
        /// <param name="tbZ">z轴位置，单位:mm</param>
        /// <param name="tbZ">r轴位置，单位:mm</param>
        /// <param name="timeout">超时等待时间ms, 0：不等待，异步运行</param>
        /// <returns></returns>
        public e_RSPErrorCode MoveXYZ(_eARM armid, double tbX, double tbY, double tbZ,int timeout)
        {
            if (!bHome)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8PA {1} {2} {3}", 
                    (int)armid, ((int)Math.Round((tbX / mmPerStepX))).ToString(),
                    ((int)Math.Round((tbY / mmPerStepY))).ToString(),
                    ((int)Math.Round((tbZ / mmPerStepZ))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid-1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }

        /// <summary>
        /// 以指定速度旋转角度，绝对角度
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="degree">绝对角度</param>
        /// <param name="DegreePerSecond">速度，单位:角度/秒</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveRAtSpeed(_eARM armid, double degree, double DegreePerSecond, int timeout = 10000)
        {
            if (!bClipperInit)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8RA {1} {2}",
                    (int)armid, ((int)Math.Round((degree / DegreePerStep))).ToString(),
                    ((int)Math.Round((DegreePerSecond / DegreePerStep))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }

            if (timeout == 0)
            {
                return e_RSPErrorCode.RSP_ERROR_NONE;
            }


            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
        /// <summary>
        /// 以指定速度张开抓手，绝对位移
        /// </summary>
        /// <param name="armid"></param>
        /// <param name="width">张开宽度，绝对位移</param>
        /// <param name="speedMMPerSecond"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public e_RSPErrorCode MoveClipperAtSpeed(_eARM armid, double width, double speedMMPerSecond, int timeout = 10000)
        {
            if (!bClipperInit)
            {
                return e_RSPErrorCode.未初始化;
            }

            if (armid >= _eARM.两个)
            {
                return e_RSPErrorCode.无效操作;
            }
            for (int i = 0; i < 3; i++)
            {
                SendCommand(string.Format("{0}8CA {1} {2}",
                    (int)armid, ((int)Math.Round((width / mmPerStepClipper))).ToString(),
                    ((int)Math.Round((speedMMPerSecond / mmPerStepClipper))).ToString()), i != 0);
                cmdACK.WaitOne(100);
                if (bACK[(int)armid - 1]) break;
            }

            if (!bACK[(int)armid - 1])
            {
                return e_RSPErrorCode.Send_fail;
            }
            if (timeout == 0)
                return e_RSPErrorCode.RSP_ERROR_NONE;

            if (timeout > 0)
            {
                while (!bActiondone[(int)armid - 1] && timeout > 0)
                {
                    Thread.Sleep(10);
                    timeout -= 10;
                }
            }

            if (timeout <= 0)
            {
                return e_RSPErrorCode.超时错误;
            }

            return _errorCode[(int)armid - 1];
        }
    
        private void ReceiveMessage()
        {
            if (!bOpen)
                return;
            byte data = 0;
            byte[] RxBuffer = { 0 };
            Listening = true;
            do
            {
                if (serialPort.BytesToRead == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                data = (byte)serialPort.ReadByte();
                if (data == ARM_PROTOCOL_STX)
                    received_count = 0;

                globalBuffer[received_count++] = data;
                if (received_count > 2 && (globalBuffer[received_count - 2] == ARM_PROTOCOL_ETX))
                {
                    RxBuffer = new byte[received_count];
                    Array.Copy(globalBuffer, RxBuffer, RxBuffer.Length);
                    if ((RxBuffer[2] == 0x31 || RxBuffer[2] == 0x32) && RxBuffer[3] == 0x38)
                    {
                        SendACK(RxBuffer[2]);
                        bool cmdDone = (RxBuffer[1] & MASK_DONE_BIT) == MASK_DONE_BIT;
                        if (cmdDone)
                        {
                            _errorCode[RxBuffer[2] - 0x31] = (e_RSPErrorCode)(RxBuffer[1] & 0x0f);
                            //cmdFinished.Set();
                            bActiondone[RxBuffer[2] - 0x31] = true;
                            if (RxBuffer.Length > 6)
                            {
                                byte[] value = new byte[RxBuffer.Length-6];
                                Array.Copy(RxBuffer, 4, value, 0, value.Length);
                                getValue = int.Parse(System.Text.Encoding.ASCII.GetString(value));
                            }
                        }
                        else
                        {
                            bool hasError = (RxBuffer[1] - 0x40) > 0;
                            if (hasError)
                            {
                                bHome = false;
                                bActiondone[RxBuffer[2] - 0x31] = true;
                                e_RSPErrorCode errorCode = (e_RSPErrorCode)(RxBuffer[1] - 0x40);
                                _errorCode[RxBuffer[2] - 0x31] = (e_RSPErrorCode)(RxBuffer[1] - 0x40);
                                string errDesc = errorCode.ToString();
                                NotifyStepLost(errDesc);
                            }
                            else
                            {
                                bACK[RxBuffer[2] - 0x31] = true;
                                cmdACK.Set();
                            }
                        }
                    }
                    else
                    {
                        received_count = 0;
                    }
                }
            } while (bOpen);
            Listening = false;
        }

        private void NotifyStepLost(string errMsg)
        {
            if (onCriticalErrorHappened != null)
                onCriticalErrorHappened(this, errMsg);
        }

        public void GetCurrentPosition(_eARM armID, ref double x, ref  double y, ref  double z)
        {
            x = -1;
            y = -1;
            z = -1;
            x = Math.Round(GetXPos(armID),1);
            y = Math.Round(GetYPos(armID),1);
            z = Math.Round(GetZPos(armID),1);
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
            
            _eARM armid;

            //FUNC_ENTER;
            byte[] data = Encoding.ASCII.GetBytes(strcmd);//获得缓存
            armid = (_eARM)(data[0] - 0x30);

            if (armid >= _eARM.两个)
            {
                //log.ErrorFormat("Wrong ARMID:%d", armid);
                return;
            }

            if (!bRepeat)
            {
                if (g_ARMSeqNum[(int)armid - 1] < MAX_SEQ_NUM)
                    g_ARMSeqNum[(int)armid - 1]++;
                else
                    g_ARMSeqNum[(int)armid - 1] = 1;
            }

            byte[] ucData = new byte[data.Length + 4];
            ucData[0] = ARM_PROTOCOL_STX;
            if (bRepeat)
                ucData[1] = (byte)(0x48 | (g_ARMSeqNum[(int)armid - 1] & MASK_SEQ));
            else
                ucData[1] = (byte)(0x40 | (g_ARMSeqNum[(int)armid - 1] & MASK_SEQ));
            for (int i = 0; i < data.Length; i++)
            {
                ucData[2 + i] = data[i];
            }
            ucData[data.Length + 2] = ARM_PROTOCOL_ETX;
            ucData[data.Length + 3] = ucData[0];
            for (int i = 1; i < data.Length + 3; i++)
                ucData[data.Length + 3] = (byte)(ucData[data.Length + 3] ^ ucData[i]);
            bACK[(int)armid - 1] = false;
            bActiondone[(int)armid - 1] = false;
            _errorCode[(int)armid-1] = e_RSPErrorCode.RSP_ERROR_NONE;
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
            if (serialPort != null && serialPort.IsOpen)
            {
                while (Listening)
                    System.Windows.Forms.Application.DoEvents();
                serialPort.Close();
                //log.Info("Port closed.");
            }

            
            bOpen = false;
            bHome = false;
        }




    }
    public enum _eARM
    {
        左臂 = 1,
        右臂,
        两个,
    }

    public enum Axis
    {
        X,
        Y,
        Z,
        R,
        Clipper
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
        X失步,//RSP_ERROR_Step_loss_X,  //Step loss detected on X-axis
        Y失步,//RSP_ERROR_Step_loss_Y,  //Step loss detected on Y-axis
        Z失步,//RSP_ERROR_Step_loss_Z,  //Step loss detected on Z-axis
        命令缓存溢出,//RSP_ERROR_Command_overflow, //Command overflow
        没液体ZX,//RSP_ERROR_NoLiquid_ZX,         //No liquid detected with ZX-command
        超范围,//RSP_ERROR_ZMove_out_of_range,   //Entered move for Z-axis out of range
        液体不足ZX,//RSP_ERROR_Not_enough_liquid_ZX,    //Not enough liquid detected with ZX-command
        没液体ZZ,//RSP_ERROR_Noliquid_ZZ,  //No liquid detected with ZZ-command
        液体不足ZZ,//RSP_ERROR_Not_enough_liquid_ZZ, //Not enough liquid detected with ZZ-command
        空吸,//RSP_ERROR_PICKUP_EMPTY,//Sensor broken
        泡沫,//RSP_ERROR_PICKUP_FOAM,
        凝块,//RSP_ERROR_PICKUP_BLOCK,
        撞臂保护,//RSP_ERROR_Collision_avoided,//Arm collision avoided
        RSP_ERROR_Reserved4,
        RSP_ERROR_Reserved5,
        RSP_ERROR_Step_loss_X_opp,  //Step loss detected on X-axis of opposing arm
        RSP_ERROR_ALIDUM_pulse_timeout,  //ALIDUM pulse time out
        RSP_ERROR_Tip_not_fetched,  //Tip not fetched (used with DiTi option)
        RSP_ERROR_Tip_crash,    //Tip crash (used with DiTi option)
        RSP_ERROR_Tip_not_clean,//Tip not clean (used with DiTi option)
        Send_fail,
    };

    public enum e_CanMotorID{
        CanMotorID_Left_x,   //左臂x
        CanMotorID_Left_y,   //左臂y
        CanMotorID_Left_z,   //左臂z
        CanMotorID_Right_x,   //右臂x
        CanMotorID_Right_y,   //右臂y
        CanMotorID_Right_z,   //右臂z
        CanMotorID_Rotate,   //右臂夹爪旋转
        CanMotorID_Clipper,   //右臂夹爪张合
        CanMotorID_Max,
    };
}
