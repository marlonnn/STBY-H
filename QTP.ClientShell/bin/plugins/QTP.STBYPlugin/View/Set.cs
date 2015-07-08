using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;

namespace QTP.STBYPlugin.View
{
    public partial class Set : Office2007Form
    {
        public Set()
        {
            InitializeComponent();
        }

        //private static Set _preSet;
        //public static Set GetInstance()
        //{
        //    if (_preSet == null)
        //    {
        //        _preSet = new Set();
        //    }
        //    return _preSet;
        //}

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EnterClick(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)//是回车
            {
                btnOK_Click(null, null);
            }
        }
    }
}
