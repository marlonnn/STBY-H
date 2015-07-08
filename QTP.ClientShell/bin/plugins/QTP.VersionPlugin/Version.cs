using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QTP.VersionPlugin
{
    public partial class Version : UserControl
    {
        public Version()
        {
            InitializeComponent();
        }

        private void btn_helpClick(object sender, EventArgs e)
        {
            //string current = System.Environment.CurrentDirectory;
            //string path = string.Format("{0}\\Config\\{1}", System.Environment.CurrentDirectory, "TSP硬件平台开发项目功能测试系统用户操作手册.pdf");
            //System.Diagnostics.Process.Start(path);
        }
    }
}
