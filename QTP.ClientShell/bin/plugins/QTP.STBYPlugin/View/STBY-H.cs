using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Summer.System.IO;
using System.Runtime.InteropServices;
using QTP.STBYPlugin.Data;
using System.Threading;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;

namespace QTP.STBYPlugin.View
{
    public partial class STBY_H : UserControl
    {
        private string file = Environment.CurrentDirectory + @"\Config\config.xml";
        private string parm_file = Environment.CurrentDirectory + @"\Config\parm_config.xml";
        private Dictionary<string, string> itemDic = new Dictionary<string, string>();//测试item/type
        private Dictionary<string, int> typeAndDelay = new Dictionary<string, int>();//测试type/delay
        private Dictionary<string, int> typeAndCount = new Dictionary<string, int>();//测试type/count

        private UInt32 m_devtype = 4;
        public static UInt32 m_bOpen = 0;

        private UInt32 m_devind = 0;//索引号 默认为0
        private UInt32 can0_index = 0;//第0路CAN
        private UInt32 can1_index = 1;//第1路CAN

        private UInt32 m_acccode = 0x00000000;//验收码
        private UInt32 m_accmask = 0xFFFFFFFF;//屏蔽码
        private byte m_timer0 = 0x00;//定时器0
        private byte m_timer1 = 0x14;//定时器1
        private byte m_filter = 0x00;//过滤方式  0：单滤波  1：双滤波
        private byte m_mode = 0x00;//模式  0：正常  1：只听
        private byte m_sendType = 0x01;//发送类型：正常发送 ；单次正常发送 01；自发自收 ；单次自发自收
        private byte m_framType = 0x01;//帧类型：标准帧00；扩展帧 01 
        private byte m_framFormat = 0x00;//帧格式：数据帧00；远程帧 01

        private Thread invokeThread;

        private object lockthis = new object();

        public static int threadParm = 0;
        public static int sendParm = 1;//datagridview界面更新发送数据数
        private int totalTestCount = 0;

        private bool isStartThread = false;//子线程是否开始

        //private volatile bool shouldStop = false;//线程是否停止标志


        #region 继电器测试

        private string realData11 = "0be 0ff 0fa 078 00 00 00 00";//实际应该收到的数据
        private string realData12 = "0be 0ff 0c8 0a1 00 00 00 00";//实际应该收到的数据
        private string realData13 = "00 00 00 00 0bc 0ff 011 0ec";//实际应该收到的数据
        private string realData14 = "00 00 00 00 0bc 0ff 0e 0c9";//实际应该收到的数据
        private string realData21 = "0fe 0ff 0a6 073 00 00 00 00";//实际应该收到的数据
        private string realData22 = "0fe 0ff 09c 0cb 00 00 00 00";//实际应该收到的数据
        private string realData23 = "00 00 00 00 0fc 0ff 04d 0e7";//实际应该收到的数据
        private string realData24 = "00 00 00 00 0fc 0ff 05a 0a3";//实际应该收到的数据
        #endregion

        private const string CAN1_422 = "CAN1_422 Test";
        private const string CAN2_422 = "CAN2_422 Test";
        private const string Realy_Flash_B = "B板卡Realy_Flash";
        private const string Realy_Flash_A = "A板卡Realy_Flash";
        private const string Slot_Test_B = "B板卡Slot Test";
        private const string Slot_Test_A = "A板卡Slot Test";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="Reserved"></param>
        /// <returns></returns>
        #region 导入CNA通信相关接口函数
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);
        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        #endregion

        public STBY_H()
        {
            InitializeComponent();
            InitalizeData();
            GetXMLData();
            InitialGridData();
            GetXMLParmData();
        }


        private void InitalizeData()
        {
            txb_testcount.Text = "3";
            txb_testedcount.Text = "0";
            SetBtnStatus(true);
        }

        /// <summary>
        /// 获取config.xml配置文件中的测试项和测试type、delay，并将其分别存储到Dictionary中
        /// </summary>
        private void GetXMLData()
        {
            var xml = XmlFileHelper.CreateFromFile(file);
            foreach (var v in xml.GetRoot().FilterChildren("STBY"))
            {
                foreach (var item in v.FilterChildren("parm"))
                {
                    itemDic[item.GetAttribute("item")] = item.GetAttribute("type");
                }
                foreach (var item in v.FilterChildren("parm"))
                {
                    typeAndDelay[item.GetAttribute("type")] = Int32.Parse(item.GetAttribute("delay"));
                }
            }
        }

        /// <summary>
        /// 获取parm_config.xml配置文件中的CAN通信参数
        /// </summary>
        private void GetXMLParmData()
        {
            var xml = XmlFileHelper.CreateFromFile(parm_file);
            foreach (var v in xml.GetRoot().FilterChildren("STBY"))
            {
                foreach (var item in v.FilterChildren("parm"))
                {
                    switch (item.GetAttribute("name"))
                    {
                        case "type":
                            m_devtype = UInt32.Parse(item.GetAttribute("value"));
                            break;
                        case "accCode":
                            m_acccode = UInt32.Parse(item.GetAttribute("value"));
                            break;
                        case "accMask":
                            m_accmask = UInt32.Parse(item.GetAttribute("value"));
                            break;
                        case "time0":
                            m_timer0 = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "time1":
                            m_timer1 = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "filter":
                            m_filter = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "mode":
                            m_mode = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "sendType":
                            m_sendType = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "framType":
                            m_framType = Byte.Parse(item.GetAttribute("value"));
                            break;
                        case "framFormat":
                            m_framFormat = Byte.Parse(item.GetAttribute("value"));
                            break;
                    }
                }
            }
        }

        private Dictionary<string, int> GetTestCountDictionary()
        {
            int sum = 0;
            Dictionary<string, int> temp = new Dictionary<string, int>();
            for (int i = 0; i < dataGridViewX1.Rows.Count; i++)
            {
                temp[dataGridViewX1.Rows[i].Cells[1].Value.ToString()] = Int32.Parse(dataGridViewX1.Rows[i].Cells[2].Value.ToString());
                sum += Int32.Parse(dataGridViewX1.Rows[i].Cells[2].Value.ToString());
            }
            if (sum == 0)//如果所有的测试次数和为0，则返回空；
            {
                temp = null;
            }
            return temp;
        }

        /// <summary>
        /// 初始化datagridview
        /// </summary>
        private void InitialGridData()
        {
            foreach (string item in itemDic.Keys)
            {
                int index = dataGridViewX1.Rows.Add();
                dataGridViewX1.Rows[index].Cells[0].Value = index + 1;
                dataGridViewX1.Rows[index].Cells[1].Value = item;
                dataGridViewX1.Rows[index].Cells[2].Value = 1;
            }
        }

        private void ClearDataGridView()
        {
            int index = 0;
            dataGridViewX1.CurrentCell = dataGridViewX1.Rows[index].Cells[0];
            foreach (string item in itemDic.Keys)
            {
                dataGridViewX1.Rows[index].Cells[3].Value = "";
                dataGridViewX1.Rows[index].Cells[4].Value = "";
                dataGridViewX1.Rows[index].Cells[5].Value = "";
                dataGridViewX1.Rows[index].DefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
                index++;
            }
        }

        /// <summary>
        /// 开始测试，打开设备，初始化CAN，启动CAN、发送数据、处理接收数据（有）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        unsafe private void btnStartTest_Click(object sender, EventArgs e)
        {
            //应该判断测试项是否都为空
            typeAndCount = GetTestCountDictionary();
            if (typeAndCount == null)
            {
                if (MessageBox.Show("还没有选择任何测试项，将无法进行测试！", "注意",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return;
                }
            }
            txb_testedcount.Text = "0";
            totalTestCount = Int32.Parse(txb_testcount.Text.ToString());
            listBox_Info.Items.Clear();
            ClearDataGridView();
            //dataGridViewX1.Rows.Clear();
            if (m_bOpen == 0)
            {
                if (!OpendeDeviceAndCAN())
                {
                    return;
                }
                //btnStartTest.Text = m_bOpen == 1 ? "正在测试" : "开始测试";
            }
            //开始发送数据
            Thread.Sleep(100);
            threadParm = 0;
            sendParm = 1;
            //测试万能钥匙需要交互
            StartTestThread();
        }

        private void StartTestThread()
        {
            SetBtnStatus(false);
            isStartThread = true;
            invokeThread = new Thread(new ThreadStart(SendData));
            invokeThread.IsBackground = true;
            invokeThread.Start();
        }

        /// <summary>
        /// 设置当前测试行，焦点指在当前行
        /// </summary>
        /// <param name="index"></param>
        private void SetCurrrentGridView(int index)
        {
            if (InvokeRequired)
            {
                var d = new Action<int>(SetCurrrentGridView);
                this.Invoke(d, index);
            }
            else
            {
                dataGridViewX1.CurrentCell = dataGridViewX1.Rows[index].Cells[0];
                dataGridViewX1.Rows[index].Cells[3].Value = sendParm;
            }
        }

        /// <summary>
        /// 测试结束时更新datagridview
        /// </summary>
        /// <param name="index"></param>
        /// <param name="flag"></param>
        private void UpdateGridView(int index, bool flag)
        {
            if (InvokeRequired)
            {
                var d = new Action<int, bool>(UpdateGridView);
                this.Invoke(d, index, flag);
            }
            else
            {
                if (flag)
                {
                    //dataGridViewX1.Rows[index].Cells[4].Value = "NA";
                    //已经测试失败了 ,就算下次测试通过了也不能表示通过；
                    if (dataGridViewX1.Rows[index].Cells[4].Value != null)
                    {
                        if (dataGridViewX1.Rows[index].Cells[4].Value.ToString().Contains("测试失败"))
                        {
                            dataGridViewX1.Rows[index].Cells[5].Value = "已经测试失败";
                            dataGridViewX1.Rows[index].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            dataGridViewX1.Rows[index].Cells[4].Value = "测试通过";
                            dataGridViewX1.Rows[index].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
                        }
                    }
                    else
                    {
                        dataGridViewX1.Rows[index].Cells[4].Value = "测试通过";
                        dataGridViewX1.Rows[index].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    //dataGridViewX1.Rows[index].Cells[4].Value = "NA";
                    //停止测试
                    //或者继续测试，但是测试失败项不能修改
                    dataGridViewX1.Rows[index].Cells[4].Value = "测试失败";
                    dataGridViewX1.Rows[index].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                }

            }
        }

        /// <summary>
        /// 格式化发送数据
        /// </summary>
        /// <param name="value">type ID</param>
        /// <param name="sendData">发送数据</param>
        /// <returns></returns>
        unsafe private VCI_CAN_OBJ FormatSendData(string value, string sendData)
        {
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.SendType = m_sendType;
            sendobj.RemoteFlag = m_framFormat;
            sendobj.ExternFlag = m_framType;
            sendobj.ID = System.Convert.ToUInt32(value, 16);
            int len = (sendData.Length + 1) / 3;
            sendobj.DataLen = System.Convert.ToByte(len);
            String strdata = sendData;
            int i = -1;
            if (i++ < len - 1)
                sendobj.Data[0] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[1] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[2] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[3] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[4] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[5] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[6] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);
            if (i++ < len - 1)
                sendobj.Data[7] = System.Convert.ToByte("0x" + strdata.Substring(i * 3, 2), 16);

            return sendobj;
        }

        private void SwitchStatus(string item)
        {
            switch (item)
            {
                case "Key A Test":
                    if (MessageBox.Show("为了保证测试的正常运行，请先将万能转换开关转至A档，然后再开始本轮测试", "注意",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        if (isStartThread)//开启了测试子线程，则停止线程
                        {
                            if (Thread.CurrentThread != null)
                            {
                                try
                                {
                                    isStartThread = false;
                                    Thread.CurrentThread.Abort();
                                }
                                catch (System.Exception ex)
                                {

                                }
                            }
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
                case "Key AUTO Test":
                    if (MessageBox.Show("为了保证测试的正常运行，请先将万能转换开关转至AUTO档，然后再开始本轮测试", "注意",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        if (isStartThread)//开启了测试子线程，则停止线程
                        {
                            if (Thread.CurrentThread != null)
                            {
                                try
                                {
                                    isStartThread = false;
                                    Thread.CurrentThread.Abort();
                                }
                                catch (System.Exception ex)
                                {

                                }
                            }
                            return;
                        }
                        else
                        {
                        }
                    }
                    break;
                case "Key B Test":
                    if (MessageBox.Show("为了保证测试的正常运行，请先将万能转换开关转至B档，然后再开始本轮测试", "注意",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        if (isStartThread)//开启了测试子线程，则停止线程
                        {
                            if (Thread.CurrentThread != null)
                            {
                                try
                                {
                                    isStartThread = false;
                                    Thread.CurrentThread.Abort();
                                }
                                catch (System.Exception ex)
                                {

                                }
                            }
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    break;
            }
        }

        private bool JudgeSwitchIsSingle(string item)
        {
            int ATestCount = 0;
            int AutoTestCount = 0;
            int BTestCount = 0;
            bool temp = false;
            switch (item)
            {
                case "Key A Test":
                    AutoTestCount = typeAndCount["Key AUTO Test"];
                    BTestCount = typeAndCount["Key B Test"];
                    if (AutoTestCount == 0 && BTestCount == 0)
                    {
                        temp = true;
                    }
                    else
                    {
                        temp = false;
                    }
                    break;
                case "Key AUTO Test":
                    ATestCount = typeAndCount["Key A Test"];
                    BTestCount = typeAndCount["Key B Test"];
                    if (ATestCount == 0 && BTestCount == 0)
                    {
                        temp = true;
                    }
                    else
                    {
                        temp = false;
                    }
                    break;
                case "Key B Test":
                    ATestCount = typeAndCount["Key A Test"];
                    AutoTestCount = typeAndCount["Key AUTO Test"];
                    if (ATestCount == 0 && AutoTestCount == 0)
                    {
                        temp = true;
                    }
                    else
                    {
                        temp = false;
                    }
                    break;
            }
            return temp;
        }

        /// <summary>
        /// 切换开始测试和停止测试按钮状态
        /// </summary>
        /// <param name="flag"></param>
        private void SetBtnStatus(bool flag)
        {
            if (InvokeRequired)
            {
                var d = new Action<bool>(SetBtnStatus);
                this.Invoke(d, flag);
            }
            else
            {
                if (flag)
                {
                    btnStartTest.Enabled = true;
                    btnStopTest.Enabled = false;
                }
                else
                {
                    btnStartTest.Enabled = false;
                    btnStopTest.Enabled = true;
                }
            }
        }

        unsafe private void SendData()
        {
            lock (lockthis)
            {
                threadParm++;
                int index = 0;
                Thread.Sleep(10);
                #region 依次测试每个Item
                foreach (string testItem in typeAndCount.Keys)
                {
                    if (typeAndCount[testItem] == 0)
                    {
                        index++;
                        continue;
                    }
                    else
                    {
                        sendParm = 1;
                        for (int i = 0; i < typeAndCount[testItem]; i++)//小循环测试次数
                        {
                            string value = itemDic[testItem];
                            SetCurrrentGridView(index);
                            sendParm++;
                            string sendData = GenerateRandomNumber(8);
                            VCI_CAN_OBJ sendobj = FormatSendData(value, sendData);
                            //判断档位，判断其他A、AUTO、B档是否测试次数中如果有两个测试次数为0,则不需要提示；
                            //测试万能钥匙时需要交互
                            if (!JudgeSwitchIsSingle(testItem))
                            {
                                SwitchStatus(testItem);
                            }

                            if (VCI_Transmit(m_devtype, m_devind, can0_index, ref sendobj, 1) == 0)
                            {
                                if (MessageBox.Show("CAN 1发送失败", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                                {
                                    //测试失败
                                    UpdateGridView(index, false);
                                }

                            }
                            Thread.Sleep(typeAndDelay[value]);
                            //判断接收到的：type、SN、Data、回踩数据
                            DealWithAllTestItem(testItem, sendData, value, index);
                        }
                        index++;
                    }


                }
                #endregion
                UpdateTestedTimes(threadParm);
                //sendParm++;//测试一轮结束，将发送数据数量加1
                if (threadParm != totalTestCount)
                {
                    StartTestThread();
                }
                else
                {
                    //测试结束
                    isStartThread = false;
                    SetBtnStatus(true);
                    //RequestStop();
                    if (MessageBox.Show("本次测试结束，是否保存测试结果信息？", "注意",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        if (ExportDataToPDFTable())
                        {
                            MessageBox.Show("导出成功！", "注意",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            MessageBox.Show("导出失败！", "注意",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        //
                    }
                    if (invokeThread != null)
                    {
                        invokeThread.Abort();
                    }
                }
            }
        }

        private void DealWithAllTestItem(string testItem, string sendData, string value, int index)
        {
            switch (testItem)
            {
                case CAN1_422:
                case CAN2_422:
                    if (DealWith422ReceiveData(can0_index, sendData, value) && DealWith422ReceiveData(can1_index, sendData, value))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                case Realy_Flash_A:
                case Realy_Flash_B:
                    if (DealWithRelayFlashReceiveData(can0_index, sendData, value) && DealWithRelayFlashReceiveData(can1_index, sendData, value))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                case Slot_Test_A:
                case Slot_Test_B:
                    if (DealWithSlotReceiveData(can0_index, sendData, value) && DealWithSlotReceiveData(can1_index, sendData, value))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                case "Key A Test":
                    if (DealWithKeyReceiveData(can0_index, sendData, value, 1) && DealWithKeyReceiveData(can1_index, sendData, value, 1))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                case "Key AUTO Test":
                    if (DealWithKeyReceiveData(can0_index, sendData, value, 2) && DealWithKeyReceiveData(can1_index, sendData, value, 2))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                case "Key B Test":
                    if (DealWithKeyReceiveData(can0_index, sendData, value, 3) && DealWithKeyReceiveData(can1_index, sendData, value, 3))
                    {
                        UpdateGridView(index, true);
                    }
                    else
                    {
                        UpdateGridView(index, false);
                    }
                    break;
                default:
                    //LED灯测试
                    //修改---> 当测试次数达到最后依次测试时，并且达到单项测试次数时；
                    //测试次数
                    if (threadParm == totalTestCount && sendParm == typeAndCount[testItem] + 1)//当测试次数到达单项总测试次数时，询问是否测试成功
                    {
                        if (MessageBox.Show("当前 测试通过吗？", "注意",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //
                            UpdateGridView(index, true);
                        }
                        else
                        {
                            UpdateGridView(index, false);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 更新已测次数
        /// </summary>
        /// <param name="count"></param>
        private void UpdateTestedTimes(int count)
        {
            if (InvokeRequired)
            {
                var d = new Action<int>(UpdateTestedTimes);
                this.Invoke(d, count);
            }
            else
            {
                txb_testedcount.Text = string.Format("{0}", count);
            }
        }

        private void ShowMsgBoxBasedOnDelay(int index, bool flag)
        {
            if (MessageBox.Show("当前 测试通过吗？", "注意",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //
                UpdateGridView(index, true);
            }
            else
            {
                UpdateGridView(index, false);
            }
        }

        /// <summary>
        /// 打开设备和初始化CAN0&CAN1
        /// </summary>
        private bool OpendeDeviceAndCAN()
        {
            #region 打开设备和初始化CAN0&CAN1
            if (VCI_OpenDevice(m_devtype, m_devind, 0) != 1)
            {
                if (MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return false;
                }
            }
            m_bOpen = 1;
            VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
            config.AccCode = m_acccode;
            config.AccMask = m_accmask;
            config.Timing0 = m_timer0;
            config.Timing1 = m_timer1;
            config.Filter = m_filter;
            config.Mode = m_mode;
            if (VCI_InitCAN(m_devtype, m_devind, can0_index, ref config) != 1)
            {
                if (MessageBox.Show("初始化CAN 1失败,请检查连接是否正确", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return false;
                }
            }
            if (VCI_InitCAN(m_devtype, m_devind, can1_index, ref config) != 1)
            {
                if (MessageBox.Show("初始化CAN 2失败,请检查连接是否正确", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return false;
                }
            }
            if (VCI_StartCAN(m_devtype, m_devind, can0_index) != 1)
            {
                if (MessageBox.Show("启动CAN 1失败,请检查连接是否正确", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return false;
                }
            }
            if (VCI_StartCAN(m_devtype, m_devind, can1_index) != 1)
            {
                if (MessageBox.Show("启动CAN 2失败,请检查连接是否正确", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    return false;
                }
            }
            return true;
            #endregion
        }

        /// <summary>
        /// 处理CAN_422测试数据，判断接收到的：type、SN、Data、回踩数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sendData"></param>
        unsafe private bool DealWith422ReceiveData(UInt32 canIndex, string sendData, string value)
        {
            String realTypeId = "";//应该收到的type和SN（理论值）
            String recTypeId = "";//实际受到的type和SN
            String recData = "";//接收到的数据
            switch (value)
            {
                case "0x00200004":
                    realTypeId = "0x00400005";
            	break;
                case "0x00400004":
                realTypeId = "0x00200005";
                break;
            }
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, canIndex);
            if (res == 0)
                return false;
            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            res = VCI_Receive(m_devtype, m_devind, canIndex, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////
            String str = "";
            String strSend = "";
            strSend += string.Format("{0}: {1} ", DateTime.Now.ToString("yyyyMMddHHmmss"), "发送的数据: ");
            strSend += " 帧ID:" + value;
            strSend += " 数据：" + sendData + "\r\n";
            ShowListBoxInfo(strSend);
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                str = string.Format("{0}: {1} ", DateTime.Now.ToString("yyyyMMddHHmmss"), "接收的数据: ");
                str += " 帧ID:0x00" + System.Convert.ToString((Int32)obj.ID, 16);

                recTypeId = string.Format("{0}{1}", "0x00", System.Convert.ToString((Int32)obj.ID, 16));
                if (!recTypeId.Contains(realTypeId))
                {
                    return false;
                }

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += " 数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[0], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[1], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[2], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[3], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[4], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[5], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[6], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[7], 16);
                    }
                }
                if (!recData.Contains(sendData))
                {
                    return false;
                }

                ShowListBoxInfo(str + recData);
            }
            Marshal.FreeHGlobal(pt);
            return true;
        }

        /// <summary>
        /// 处理继电器测试数据，判断接收到的：type、SN、Data、回踩数据
        /// </summary>
        /// <param name="canIndex"></param>
        /// <param name="sendData"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        unsafe private bool DealWithRelayFlashReceiveData(UInt32 canIndex, string sendData, string value)
        {
            int can0_index_count = 0;
            int can1_index_count = 0;
            string recTypeId = "";//实际受到的type和SN
            string recData = "";//接收到的数据
            string realData = "";

            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, canIndex);
            if (res == 0)
                return false;
            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            res = VCI_Receive(m_devtype, m_devind, canIndex, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////
            String str = "";
            String strSend = "";
            strSend += string.Format("{0}: {1} ", DateTime.Now.ToString("yyyyMMddHHmmss"), "发送的数据: ");
            strSend += " 帧ID:" + value;
            strSend += " 数据：" + sendData + "\r\n";
            ShowListBoxInfo(strSend);

            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                DateTime time = DateTime.Now;
                string currentTime = time.ToString("yyyyMMddHHmmss");

                str = string.Format("{0}: {1} ", currentTime, "接收的数据: ");
                str += " 帧ID:0x00" + System.Convert.ToString((Int32)obj.ID, 16);

                recTypeId = string.Format("{0}{1}", "0x00", System.Convert.ToString((Int32)obj.ID, 16));
                switch (recTypeId)
                {
                    case "0x00202007":
                        realData = realData11;
                        can0_index_count++;
                        break;
                    case "0x00204007":
                        realData = realData12;
                        can0_index_count++;
                        break;
                    case "0x00206007":
                        realData = realData13;
                        can0_index_count++;
                        break;
                    case "0x00208007":
                        realData = realData14;
                        can0_index_count++;
                        break;
                    case "0x00402007":
                        realData = realData21;
                        can1_index_count++;
                        break;
                    case "0x00404007":
                        realData = realData22;
                        can1_index_count++;
                        break;
                    case "0x00406007":
                        realData = realData23;
                        can1_index_count++;
                        break;
                    case "0x00408007":
                        realData = realData24;
                        can1_index_count++;
                        break;
                }
                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += " 数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    recData = "";
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[0], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[1], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[2], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[3], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[4], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[5], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[6], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[7], 16);
                    }
                }
                if (!recData.Contains(realData))
                {
                    return false;
                }

                ShowListBoxInfo(str + recData);
            }
            if (value.Contains("0x00200006"))
            {
                if (can0_index_count != 4)
                {
                    return false;
                }
            }
            else if (value.Contains("0x00400006"))
            {
                if (can1_index_count != 4)
                {
                    return false;
                }
            }
            Marshal.FreeHGlobal(pt);
            return true;
        }

        /// <summary>
        /// 处理槽道测试数据，判断接收到的：type、SN、Data、回踩数据
        /// </summary>
        /// <param name="canIndex"></param>
        /// <param name="sendData"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        unsafe private bool DealWithSlotReceiveData(UInt32 canIndex, string sendData, string value)
        {
            String realTypeId = "";//应该收到的type和SN（理论值）
            String recTypeId = "";//实际受到的type和SN
            String realData = "";
            String recData = "";//接收到的数据
            if (value.Contains("0x0020000a"))
            {
                realTypeId = "0x0020000b";
                realData = "01 00 00 00 00 00 00 00";
            }
            else if (value.Contains("0x0040000a"))
            {
                realTypeId = "0x0040000b";
                realData = "02 00 00 00 00 00 00 00";
            }
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, canIndex);
            if (res == 0)
                return false;
            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            res = VCI_Receive(m_devtype, m_devind, canIndex, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////
            String str = "";
            String strSend = "";
            strSend += string.Format("{0}: {1} ", DateTime.Now.ToString("yyyyMMddHHmmss"), "发送的数据: ");
            strSend += " 帧ID:" + value;
            strSend += " 数据：" + sendData + "\r\n";
            ShowListBoxInfo(strSend);

            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                DateTime time = DateTime.Now;
                string currentTime = time.ToString("yyyyMMddHHmmss");

                str = string.Format("{0}: {1} ", currentTime, "接收的数据: ");
                str += " 帧ID:0x00" + System.Convert.ToString((Int32)obj.ID, 16);
                //str += "  帧格式:";
                //if (obj.RemoteFlag == 0)
                //    str += "数据帧 ";
                //else
                //    str += "远程帧 ";
                //if (obj.ExternFlag == 0)
                //    str += "标准帧 ";
                //else
                //    str += "扩展帧 ";

                recTypeId = string.Format("{0}{1}", "0x00", System.Convert.ToString((Int32)obj.ID, 16));
                if (!recTypeId.Contains(realTypeId))
                {
                    return false;
                }

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += " 数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[0], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[1], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[2], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[3], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[4], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[5], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[6], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[7], 16);
                    }
                }
                if (!recData.Contains(realData))
                {
                    return false;
                }

                ShowListBoxInfo(str + recData);
            }
            Marshal.FreeHGlobal(pt);
            return true;
        }

        /// <summary>
        /// 处理万能转换开关测试数据，判断接收到的：type、SN、Data、回踩数据
        /// </summary>
        /// <param name="canIndex"></param>
        /// <param name="sendData"></param>
        /// <param name="value"></param>
        /// <param name="threadParm"></param>
        /// <returns></returns>
        unsafe private bool DealWithKeyReceiveData(UInt32 canIndex, string sendData, string value, int threadParm)
        {
            String realTypeId = "0x00200009";//应该收到的type和SN（理论值）
            String recTypeId = "";//实际受到的type和SN
            String realData = "";
            String recData = "";//接收到的数据
            switch (threadParm)
            {
                case 1://A 02
                    realData = "02 00 00 00 00 00 00 00";
                    break;
                case 2://AUTO 03
                    realData = "03 00 00 00 00 00 00 00";
                    break;
                case 3://B 01
                    realData = "01 00 00 00 00 00 00 00";
                    break;
            }
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, canIndex);
            if (res == 0)
                return false;
            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            res = VCI_Receive(m_devtype, m_devind, canIndex, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////
            String str = "";
            String strSend = "";
            strSend += string.Format("{0}: {1} ", DateTime.Now.ToString("yyyyMMddHHmmss"), "发送的数据: ");
            strSend += " 帧ID:" + value;
            strSend += " 数据：" + sendData + "\r\n";
            ShowListBoxInfo(strSend);

            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                DateTime time = DateTime.Now;
                string currentTime = time.ToString("yyyyMMddHHmmss");

                str = string.Format("{0}: {1} ", currentTime, "接收的数据: ");
                str += " 帧ID:0x00" + System.Convert.ToString((Int32)obj.ID, 16);
                //str += "  帧格式:";
                //if (obj.RemoteFlag == 0)
                //    str += "数据帧 ";
                //else
                //    str += "远程帧 ";
                //if (obj.ExternFlag == 0)
                //    str += "标准帧 ";
                //else
                //    str += "扩展帧 ";

                recTypeId = string.Format("{0}{1}", "0x00", System.Convert.ToString((Int32)obj.ID, 16));
                if (!recTypeId.Contains(realTypeId))
                {
                    return false;
                }

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += " 数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[0], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[1], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[2], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[3], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[4], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[5], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[6], 16) + " ";
                    }
                    if (j++ < len)
                    {
                        recData += "0" + System.Convert.ToString(obj.Data[7], 16);
                    }
                }
                if (!recData.Contains(realData))
                {
                    return false;
                }

                ShowListBoxInfo(str + recData);
            }
            Marshal.FreeHGlobal(pt);
            return true;
        }

        private void ShowListBoxInfo(string info)
        {
            if (InvokeRequired)
            {
                var d = new Action<string>(ShowListBoxInfo);
                this.Invoke(d, info);
            }
            else
            {
                listBox_Info.Items.Add(info);
                listBox_Info.SelectedIndex = listBox_Info.Items.Count - 1;
            }
        }

        /// <summary>
        /// 将datagirdview中的数据导出到pdf文件中
        /// </summary>
        /// <returns></returns>
        private bool ExportDataToPDFTable()
        {
            bool rt = true;
            Document doc = new Document(iTextSharp.text.PageSize.A4, 25, 25, 25, 25);
            string path = System.Environment.CurrentDirectory;
            if(!Directory.Exists(string.Format("{0}/Report",path)))
            {
                Directory.CreateDirectory(string.Format("{0}/Report",path));
            }
            string pdfFilePath = string.Format("{0}/Report/{1}.pdf",
                System.Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMddHHmmss"));
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath, FileMode.Create));
            doc.AddCreationDate();
            doc.Open();
            BaseFont bfSon = BaseFont.CreateFont(@"D:\CodeRepo\STBY-H\QTP.ClientShell\font\simsun.ttf",
                BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font8 = new Font(bfSon, 8);

            Paragraph paragraph = new Paragraph("CASCO STBY-H 综合测试", font8);
            Paragraph pTotalTest = new Paragraph(string.Format("总测试次数：{0}", txb_testcount.Text.ToString()), font8);
            Paragraph pHaveTest = new Paragraph(string.Format("已测试次数：{0}", txb_testedcount.Text.ToString()), font8);

            DataTable dt = GetDgvToTable(dataGridViewX1);
            try
            {
                if (dt != null)
                {
                    int numColumns = dt.Columns.Count;
                    PdfPTable pdfTable = new PdfPTable(numColumns);
                    PdfPCell pdfPCell = null;

                    pdfPCell = new PdfPCell(new Phrase(new Chunk("序号", font8)));
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("测试项", font8)));
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("单项测试次数", font8)));
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("测试次数", font8)));
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("测试结果", font8)));
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("备注", font8)));
                    pdfTable.AddCell(pdfPCell);

                    for (int rows = 0; rows < dt.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dt.Columns.Count; column++)
                        {
                            pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));
                            pdfTable.AddCell(pdfPCell);
                        }
                    }
                    pdfTable.SpacingBefore = 15f;
                    doc.Add(paragraph);
                    doc.Add(pTotalTest);
                    doc.Add(pHaveTest);
                    doc.Add(pdfTable);
                }
            }
            catch (System.Exception ex)
            {
                rt = false;
            }
            finally
            {
                doc.Close();
            }
            return rt;
        }

        /// <summary>
        /// 保存数据到pdf文档中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //将表格导出

            if (MessageBox.Show("是否导出测试信息？", "注意",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (ExportDataToPDFTable())
                {
                    MessageBox.Show("导出成功！", "注意",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("导出失败！", "注意",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// 将datagridview中的数据转换成datatable数据结构
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        private DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            //列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            //循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 产生len长度的随机数
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private string GenerateRandomNumber(int len)
        {
            string[] temp = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09" };
            StringBuilder newRandom = new StringBuilder(8);
            Random rd = new Random();
            for (int i = 0; i < len; i++)
            {
                if (i == len - 1)
                {
                    newRandom.Append(temp[rd.Next(9)]);
                }
                else
                {
                    newRandom.Append(temp[rd.Next(9)] + " ");
                }
            }

            return newRandom.ToString();
        }

        private void btnStopTest_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                if (isStartThread)
                {
                    //
                    if (invokeThread != null)
                    {
                        invokeThread.Abort();
                    }
                }
                SetBtnStatus(true);
                VCI_ClearBuffer(m_devtype, m_devind, can0_index);
                VCI_ClearBuffer(m_devtype, m_devind, can1_index);

                VCI_CloseDevice(m_devtype, can0_index);
                VCI_CloseDevice(m_devtype, can1_index);
                m_bOpen = 0;
            }
        }

    }
}
