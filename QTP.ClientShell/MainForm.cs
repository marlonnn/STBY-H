using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UIShell.OSGi;
using UIShell.OSGi.Core.Service;
using System.Xml;

namespace QTP.ClientShell
{
    public partial class MainForm : Form
    {
        private BundleRuntime bundleRuntime;
        private const string VPI3Extension = "QTP.OSGI";
        public MainForm()
        {
            InitializeComponent();
            using (bundleRuntime = new BundleRuntime())//使用using结束时，回收所有using段内的内存
            {
                bundleRuntime.Start();
                HandleExtension();
            }
        }

        /// <summary>
        /// Handle extension information from bundle
        /// </summary>
        void HandleExtension()
        {
            IExtensionManager extensionManager = bundleRuntime.Framework.ServiceContainer.GetFirstOrDefaultService<IExtensionManager>();
            // 处理UIShell.OSGi.MainShell扩展点。
            List<Extension> extensions = extensionManager.GetExtensions("QTP.OSGI");
            if (extensions != null && extensions.Count > 0)
            {
                foreach (Extension extension in extensions)
                {
                    Extension ex = extension;
                    List<XmlNode> data = ex.Data;

                    if (data != null && data.Count > 0)
                    {
                        foreach (XmlNode topNode in data)
                        {
                            if (topNode.Attributes["Page"] != null)
                            {
                                // 从扩展点获取扩展插件定义的用户控件，然后添加到MainForm中。
                                string mainShellClassName = topNode.Attributes["Page"].Value;
                                Type type = ex.Owner.LoadClass(mainShellClassName);
                                Control control = (Control)System.Activator.CreateInstance(type);
                                TabPage tabPage = new TabPage(mainShellClassName = topNode.Attributes["Title"].Value);
                                tabPage.Controls.Add(control);
                                tabControl1.TabPages.Add(tabPage);
                                control.Dock = DockStyle.Fill;
                                control.Visible = true;
                                //UserControl uc = (UserControl)System.Activator.CreateInstance(type);
                                //this.Controls.Add(control);
                            }
                        }
                        //XmlNode topNode = data[0];

                    }
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bundleRuntime.Stop();
        }
    }
}
