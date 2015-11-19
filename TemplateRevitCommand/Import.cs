using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Revit_ImportImages
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Import : IExternalCommand
    {
        public static Element selElement;
        public static UIApplication appRevit;
        ProcessManager pm = new ProcessManager();
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            appRevit = commandData.Application;
            UIDocument uidoc = appRevit.ActiveUIDocument;
            Document doc = uidoc.Document;
            string nameFile = pm.GetImageFromView(uidoc, doc.ActiveView,false);
            if (nameFile == "") return Result.Succeeded;
            pm.ImportimageToProject(nameFile);
            return Result.Succeeded;
        }
    }
}
