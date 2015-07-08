using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;
using UIShell.BundleManagementService;

namespace QTP.BundleManagerPlugin
{
    public class Activator : IBundleActivator
    {
        public static  ServiceTracker<IBundleManagementService> BundleManagementServiceTracker { get; private set; }

        public void Start(IBundleContext context)
        {
            //todo:
            BundleManagementServiceTracker = new ServiceTracker<IBundleManagementService>(context);
        }

        public void Stop(IBundleContext context)
        {
            //todo:
        }

        //public IBundleManagementService BundleMService
        //{
        //    get { return bundleMService; }
        //    set { bundleMService = value; }
        //}
    }
}
