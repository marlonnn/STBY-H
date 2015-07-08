using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;
using QTP.Sqlite3Plugin.Implement;
using QTP.Sqlite3Plugin.Interface;

namespace QTP.Sqlite3Plugin
{
    public class Activator : IBundleActivator
    {
        public void Start(IBundleContext context)
        {
            //todo:
            context.AddService<ISqlite3Helper>(new Sqlite3Helper());
        }

        public void Stop(IBundleContext context)
        {
            //todo:
        }
    }
}
