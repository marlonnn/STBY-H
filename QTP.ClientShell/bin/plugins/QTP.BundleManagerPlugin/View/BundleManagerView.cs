using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIShell.OSGi;

namespace QTP.BundleManagerPlugin
{
    public partial class BundleManagerView : UserControl
    {
        private Dictionary<int, long> bundleDictionary = new Dictionary<int, long>();
        private Dictionary<int, IBundle> bundleDic = new Dictionary<int, IBundle>();
        public BundleManagerView()
        {
            InitializeComponent();
            BindBundlesData();
        }
        private void BindBundlesData()
        {
            if (Activator.BundleManagementServiceTracker.IsServiceAvailable)
            {
                var bundleMService = Activator.BundleManagementServiceTracker.DefaultOrFirstService;
                if (bundleMService != null)
                {
                    var bundles = bundleMService.GetLocalBundles();
                    foreach (var bundle in bundles)
                    {
                        int index = dataGridViewX1.Rows.Add();
                        bundleDictionary[index] = bundle.BundleID;
                        bundleDic[index] = bundle;
                        dataGridViewX1.Rows[index].Cells[1].Value = bundle.Name;
                        dataGridViewX1.Rows[index].Cells[2].Value = bundle.SymbolicName;
                        dataGridViewX1.Rows[index].Cells[3].Value = bundle.Version.ToString();
                        dataGridViewX1.Rows[index].Cells[4].Value = bundle.State.ToString();
                        dataGridViewX1.Rows[index].Cells[5].Value = bundle.StartLevel;
                    }
                }

            }
        }

        private void btn_manager_click(object sender, EventArgs e)
        {
            var bundleMService = Activator.BundleManagementServiceTracker.DefaultOrFirstService;
            //IBundleManagementService bundleMService = Activator.BundleManagementServiceTracker.DefaultOrFirstService;
            var type = bundleMService.GetType();
            for (int i = 0; i < dataGridViewX1.Rows.Count; i++ )
            {
                if((bool)dataGridViewX1.Rows[i].Cells[0].EditedFormattedValue == true)
                {
                    if (bundleMService != null && bundleDic.ContainsKey(i))
                    {
                        try
                        {
                            bool allowStopped = bundleMService.IsBundleAllowedToStop((bundleDic[i] as IBundle).BundleID);
                            bundleMService.StopBundle((bundleDic[i] as IBundle).BundleID);
                        }
                        catch (BundleException ex1)
                        {
                        	
                        }
                        catch(NullReferenceException ex2)
                        {

                        }
                    }
                }
            }
            BindBundlesData();

        }

        private void btn_uninstall_click(object sender, EventArgs e)
        {

        }

        private void btn_refresh_click(object sender, EventArgs e)
        {
            BindBundlesData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == -1 || e.RowIndex == -1)
            {
                //string name = dataGridViewX1.Rows[e.RowIndex].Cells[1].Value.ToString();
                return;
            }
            int x = dataGridViewX1.CurrentCell.ColumnIndex;
            if(x == 0)
            {
                for (int i = 0; i < dataGridViewX1.Rows.Count;i++ )
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridViewX1.Rows[i].Cells[0];
                    checkCell.Value = false;
                }
                DataGridViewCheckBoxCell ifCheck = (DataGridViewCheckBoxCell)dataGridViewX1.Rows[e.RowIndex].Cells[0];
                ifCheck.Value = true;
            }
            string name = dataGridViewX1.Rows[e.RowIndex].Cells[1].Value.ToString();
            tbx_current_bundle.Text = name;
        }
    }
}
