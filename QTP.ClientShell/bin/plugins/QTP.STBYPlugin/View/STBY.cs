using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using QTP.STBYPlugin.Data;

namespace QTP.STBYPlugin.View
{
    public partial class STBY : UserControl
    {
        public Set mSet = null;
        private UInt32 m_devtype = 4;//USBCAN2

        private const int VCI_USBCAN1 = 3;
        private const int VCI_USBCAN2 = 4;

        private UInt32 m_bOpen = 0;
        private UInt32 m_devind = 0;
        private UInt32 m_canind = 0;

        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[50];

        UInt32[] m_arrdevtype = new UInt32[20];
        private string[] items = new string[] { "LED Test", "LED On", "LED Off", "CAN_422", "Realy_Flash","Key", "Slot" };

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

        public STBY()
        {
            InitializeComponent();
            InitializeData();
            InitialGridData(items);
        }

        /// <summary>
        /// 初始化comboBox
        /// </summary>
        private void InitializeData()
        {
            Int32 curindex = 0;
            comboBox_devtype.Items.Clear();

            curindex = comboBox_devtype.Items.Add("USBCAN1(I)");
            m_arrdevtype[curindex] = VCI_USBCAN1;

            curindex = comboBox_devtype.Items.Add("USBCAN2(II)");
            m_arrdevtype[curindex] = VCI_USBCAN2;

            comboBox_devtype.SelectedIndex = 1;
            comboBox_DevIndex.SelectedIndex = 0;
            comboBox_CANIndex.SelectedIndex = 0;
            comboBox_Filter.SelectedIndex = 0;
            comboBox_Mode.SelectedIndex = 0;
            comboBox_SendType.SelectedIndex = 0;
            comboBox_FrameType.SelectedIndex = 0;
            comboBox_FrameFormat.SelectedIndex = 0;
        }

        private void InitialGridData(string[] items)
        {
            foreach (string item in items)
            {
                int index = dataGridViewX1.Rows.Add();
                dataGridViewX1.Rows[index].Cells[0].Value = index +1;
                dataGridViewX1.Rows[index].Cells[1].Value = item;
                //DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridViewX1.Rows[index].Cells[2];
                dataGridViewX1.Rows[index].Cells[2].Value = this.count.Items[1];
                dataGridViewX1.Rows[index].Cells[3].Value = string.Format("{0}{1}", "0x000000", (index + 1).ToString());
                //int i = this.count.Items.IndexOf(dataGridViewX1.Rows[index].Cells[2].Value);

                //cb.SelectedIndex = 1;
                //ComboBox cb = (ComboBox)dataGridViewX1.Rows[index].Cells[2].Value;
                //cb.SelectedIndex = 0;
                //this.count.Items.IndexOf(dataGridViewX1.Rows[index].Cells[2].Value);
                //this.count.Items[index] = 1;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                VCI_CloseDevice(m_devtype, m_devind);
                m_bOpen = 0;
            }
            else
            {
                m_devtype = m_arrdevtype[comboBox_devtype.SelectedIndex];//comboBox_devtype
                m_devind = (UInt32)comboBox_DevIndex.SelectedIndex;
                m_canind = (UInt32)comboBox_CANIndex.SelectedIndex;
                if (VCI_OpenDevice(m_devtype, m_devind, 0) == 0)
                {
                    MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                m_bOpen = 1;
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = System.Convert.ToUInt32("0x" + textBox_AccCode.Text, 16);
                config.AccMask = System.Convert.ToUInt32("0x" + textBox_AccMask.Text, 16);
                config.Timing0 = System.Convert.ToByte("0x" + textBox_Time0.Text, 16);
                config.Timing1 = System.Convert.ToByte("0x" + textBox_Time1.Text, 16);
                config.Filter = (Byte)comboBox_Filter.SelectedIndex;
                config.Mode = (Byte)comboBox_Mode.SelectedIndex;
                VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);
            }
            btnConnect.Text = m_bOpen == 1 ? "断开" : "连接";
            timer_rec.Enabled = m_bOpen == 1 ? true : false;
        }

        /// <summary>
        /// 启动CAN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartCAN_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
            {
                MessageBox.Show("请检查CAN设备是否连接正确", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //VCI_StartCAN(m_devtype, m_devind, m_canind);
            if (VCI_StartCAN(m_devtype, m_devind, m_canind) == 0)
            {
                MessageBox.Show("启动CAN失败,请检查连接是否正确", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        unsafe private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
            {
                MessageBox.Show("请检查CAN设备是否已打开", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            //sendobj.Init();
            sendobj.SendType = (byte)comboBox_SendType.SelectedIndex;
            sendobj.RemoteFlag = (byte)comboBox_FrameFormat.SelectedIndex;
            sendobj.ExternFlag = (byte)comboBox_FrameType.SelectedIndex;
            sendobj.ID = System.Convert.ToUInt32("0x" + textBox_ID.Text, 16);
            int len = (textBox_Data.Text.Length + 1) / 3;
            sendobj.DataLen = System.Convert.ToByte(len);
            String strdata = textBox_Data.Text;
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

            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        unsafe private void timer_rec_Tick(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
            if (res == 0)
                return;
            /////////////////////////////////////
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            res = VCI_Receive(m_devtype, m_devind, m_canind, pt, con_maxlen, 100);
            ////////////////////////////////////////////////////////

            String str = "";
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                str = "接收到数据: ";
                str += "  帧ID:0x" + System.Convert.ToString((Int32)obj.ID, 16);
                str += "  帧格式:";
                if (obj.RemoteFlag == 0)
                    str += "数据帧 ";
                else
                    str += "远程帧 ";
                if (obj.ExternFlag == 0)
                    str += "标准帧 ";
                else
                    str += "扩展帧 ";

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += "数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[0], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[1], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[2], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[3], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[4], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[5], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[6], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[7], 16);

                }

                listBox_Info.Items.Add(str);
                listBox_Info.SelectedIndex = listBox_Info.Items.Count - 1;
            }
            Marshal.FreeHGlobal(pt);
        }

    }
}
