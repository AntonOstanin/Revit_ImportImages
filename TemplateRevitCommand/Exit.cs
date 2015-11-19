using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Revit_ImportImages
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Exit : IExternalCommand
    {
        public static Element selElement;
        public static UIApplication appRevit;
        ProcessManager pm = new ProcessManager();
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            string killNameProcess = "Revit";
            Process[] processes = Process.GetProcessesByName(killNameProcess);
            processes.First().Kill();
            return Result.Succeeded;
        }
    }
}
