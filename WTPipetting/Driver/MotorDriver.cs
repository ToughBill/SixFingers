
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MotorDriver
{

    public class RSPDriver
    {
        private static SerialPort spMain = new SerialPort();
        private StringBuilder builder = new StringBuilder();
        private long received_count = 0;//接收计数  
        private long send_count = 0;//发送计数  
        private bool bAutoTest = false;
        private bool bAutoTestDone = false;
        private static int totaltest = 0;
        private static int successtest = 0;
        private static int failtest = 0;
        private static byte[] g_ARMSeqNum = new byte[] { 0, 0,};
        private byte    MAX_SEQ_NUM         = 7;
        private byte    MASK_SEQ            = 0x07;
        private byte    ARM_UART_TIMEOUT    = 100; //100ms
        private byte    ARM_UART_TRYCNT     =3;
        private byte    MASK_IVA_BIT        = 0x20;
        private byte    MASK_DONE_BIT       = 0x10;
        private byte    MASK_REP_BIT        = 0x08;
        private byte    ARM_PROTOCOL_STX    = 0x02;
        private byte    ARM_PROTOCOL_ETX    = 0x03;

        private bool bACK = false;

        private enum e_RSPErrorCode{
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
        };

        private enum  _eARM
        {
            左臂=1,
            右臂,
            两个,
        }

        public RSPDriver()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox2.Items.AddRange(ports);
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
            comboBox1.SelectedIndex = 0;
            //初始化SerialPort对象  
            //spMain.NewLine = "UU";
            spMain.RtsEnable = true;//根据实际情况吧。  
            //添加事件注册  
            spMain.DataReceived += spMain_DataReceived;
        }

        private void ParseCanProtocal()
        {

        }

        static string RemoveChars(string str, string remove)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(remove))
            {
                throw new ArgumentException("....");
            }

            StringBuilder result = new StringBuilder(str.Length);

            Dictionary<char, bool> dictionary = new Dictionary<char, bool>();

            foreach (char ch in remove)
            {
                dictionary[ch] = true;
            }

            foreach (char ch in str)
            {
                if (!dictionary.ContainsKey(ch))
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }

        private byte[] RxBuf = new byte[1024];
        void spMain_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //CanMsg RxMessage = new CanMsg();
            byte[] buf = new byte[50];//声明一个临时数组存储当前来的串口数据  
            int n = 0;
            bool bDone = false;
            byte data = 0;
            Byte checksum = 0;
            byte[] RxBuffer = { 0 };
            int rxcnt = 0;
            int totalbytes = 21;
            byte contrlbyte = 0;

            do
            {
                n = spMain.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
                data = (byte)spMain.ReadByte();//读取缓冲数据  
                if (data == ARM_PROTOCOL_STX) received_count = 0;

                RxBuf[received_count++] = data;

                if (received_count>2 && (RxBuf[received_count - 2] == ARM_PROTOCOL_ETX)) break;

            } while (true);


            RxBuffer = new byte[received_count];

            Array.Copy(RxBuf, RxBuffer, RxBuffer.Length);
            builder.Clear();//清除字符串构造器的内容  

            //controlbyte
            if ((RxBuffer[1] & MASK_DONE_BIT) == 0 && (RxBuffer[1] - 0x40) > 0)
            {
                string err = ((e_RSPErrorCode)(RxBuffer[1] - 0x40)).ToString();
                MessageBox.Show(err, "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            bACK = true;

            this.Invoke((EventHandler)(delegate
            {
                //依次的拼接出16进制字符串  
                builder.Append("R:");
                //foreach (byte b in buf)
                for (int i = 0; i < received_count; i++)
                {
                    builder.Append(RxBuffer[i].ToString("X2") + " ");
                }
                this.listBox3.Items.Add(builder.ToString());
            }));

        }

        private void CtrlsEnable(bool bEnable)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //根据当前串口对象，来判断操作  
            if (spMain.IsOpen)
            {
                //打开时点击，则关闭串口  
                spMain.Close();
                //button1.Enabled = false;
                CtrlsEnable(false);
            }
            else
            {
                //关闭时点击，则设置好端口，波特率后打开  
                spMain.PortName = comboBox2.Text;
                spMain.BaudRate = int.Parse(comboBox1.Text);
                try
                {
                    spMain.Open();
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。  
                    spMain = new SerialPort();
                    //现实异常信息给客户。  
                    MessageBox.Show(ex.Message);
                }
                //button1.Enabled = true;
                CtrlsEnable(true);
            }
            //设置按钮的状态  
            button1.Text = spMain.IsOpen ? "关闭串口" : "打开串口";
            //buttonSend.Enabled = spMain.IsOpen;  
        }


        private void ComSend(string strcmd, bool bRepeat)
        {
            _eARM armid;

            //FUNC_ENTER;
            byte[] data = Encoding.ASCII.GetBytes(strcmd);//获得缓存
            armid = (_eARM)(data[0]-0x30);

            if(armid>=_eARM.两个)
            {
                Console.WriteLine("Wrong ARMID:%d", armid);
                return;
            }

            if(bRepeat==false)
            {
                if (g_ARMSeqNum[(int)armid-1] < MAX_SEQ_NUM)
                    g_ARMSeqNum[(int)armid-1]++;
                else
                    g_ARMSeqNum[(int)armid-1] = 1;
            }

            byte[] ucData = new byte[data.Length+4];
            ucData[0]=ARM_PROTOCOL_STX;
            if(bRepeat)
                ucData[1] =(byte) (0x48 | (g_ARMSeqNum[(int)armid-1] & MASK_SEQ));
            else
                ucData[1] = (byte)(0x40 | (g_ARMSeqNum[(int)armid-1] & MASK_SEQ));
            for(int i =0; i< data.Length; i++)
            {
                ucData[2 + i] = data[i];
            }
            ucData[data .Length+ 2] = ARM_PROTOCOL_ETX;
            ucData[data.Length + 3] = ucData[0];
            for (int i = 1; i < data.Length + 3; i++)
                ucData[data.Length + 3] = (byte)(ucData[data.Length + 3] ^ ucData[i]);
            spMain.Write(ucData, 0, ucData.Length);
            builder.Clear();
            //依次的拼接出16进制字符串  
            builder.Append("T:");
            foreach (byte b in ucData)
            {
                builder.Append(b.ToString("X2") + " ");
            }
            listBox3.Items.Add(builder.ToString());
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

        private void dataGridViewNodeList_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bACK = false;
            ComSend("18PI", false);
            while (bACK == false) ;
            bACK = false;
            ComSend("28PI", false);
            //while (bACK == false) ;
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ComSend("28PI", false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("18PA {0} {1} {2}", tbX.Text, tbY.Text, tbZ.Text), false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("28PA {0} {1} {2}", tbX.Text, tbY.Text, tbZ.Text), false);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("18ZX {0} {1} {2}", tbStart.Text, tbSubMerge.Text, tbEnd.Text), false);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("28ZX {0} {1} {2}", tbStart.Text, tbSubMerge.Text, tbEnd.Text), false);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("18OT {0} {1} {2}", tbX.Text, tbY.Text, tbZ.Text), false);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ComSend(string.Format("28OT {0} {1} {2}", tbX.Text, tbY.Text, tbZ.Text), false);
        }


    }

}
