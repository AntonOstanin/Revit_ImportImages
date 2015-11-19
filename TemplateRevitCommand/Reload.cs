using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit_ImportImages
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Reload : IExternalCommand
    {
       public static UIApplication appRevit;
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            appRevit = commandData.Application;
            ProcessManager pm = new ProcessManager();
            pm.ReloadImages();
            return Result.Succeeded;
        }

       
    }
}
