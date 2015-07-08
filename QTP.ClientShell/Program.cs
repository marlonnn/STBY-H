using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using UIShell.OSGi;
using System.Reflection;
using UIShell.PageFlowService;
using System.Threading;
using System.Runtime.Serialization;
using QTP.ClientShell.Properties;
using UIShell.OSGi.Utility;
using System.Configuration;
using UIShell.OSGi.Logging;
using UIShell.iOpenWorks.Bootstrapper;

namespace QTP.ClientShell
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //UpdateCore();
            RunApplication();
        }

        private static void RunApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //const string PAGE_NAME_LOGIN = "LoginPage";

            FileLogUtility.SetFileLogEnabled(true);
            FileLogUtility.SetLogFileName("log.txt");
            FileLogUtility.SetLogLevel(LogLevel);
            FileLogUtility.SetMaxFileSizeByMB(MaxLogFileSize);
            FileLogUtility.SetCreateNewFileOnMaxSize(CreateNewLogFileOnMaxSize);
            Application.Run(new MainForm());

        }

        /// <summary>
        /// 日志级别。
        /// </summary>
        private static LogLevel LogLevel
        {
            get
            {
                string level = ConfigurationSettings.AppSettings["LogLevel"];
                if (!string.IsNullOrEmpty(level))
                {
                    try
                    {
                        object result = Enum.Parse(typeof(LogLevel), level);
                        if (result != null)
                        {
                            return (LogLevel)result;
                        }
                    }
                    catch { }
                }
                return LogLevel.Debug;
            }
        }

        /// <summary>
        /// 日志文件限制的大小。
        /// </summary>
        private static int MaxLogFileSize
        {
            get
            {
                string size = ConfigurationSettings.AppSettings["MaxLogFileSize"];
                if (!string.IsNullOrEmpty(size))
                {
                    try
                    {
                        return int.Parse(size);
                    }
                    catch { }
                }

                return 10;
            }
        }

        /// <summary>
        /// 当日志大小超过限制时，是否新建一个。
        /// </summary>
        private static bool CreateNewLogFileOnMaxSize
        {
            get
            {
                string createNew = ConfigurationSettings.AppSettings["CreateNewLogFileOnMaxSize"];
                if (!string.IsNullOrEmpty(createNew))
                {
                    try
                    {
                        return bool.Parse(createNew);
                    }
                    catch { }
                }

                return false;
            }
        }

        /// <summary>
        /// 是否启用内核自动更新。
        /// </summary>
        private static bool AutoUpdateCoreFiles
        {
            get
            {
                string autoUpdateCoreFiles = ConfigurationSettings.AppSettings["AutoUpdateCoreFiles"];
                if (!string.IsNullOrEmpty(autoUpdateCoreFiles))
                {
                    try
                    {
                        return bool.Parse(autoUpdateCoreFiles);
                    }
                    catch { }
                }

                return false;
            }
        }
    }
}
