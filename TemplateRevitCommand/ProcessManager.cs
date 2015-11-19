using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revit_ImportImages
{
    class ProcessManager
    {
        //
        public string GetImageFromView(UIDocument uidoc, Autodesk.Revit.DB.View view, bool isReload)
        {
            if (!isReload)
            {
                Selection choices = uidoc.Selection;
                try
                {
                    Reference hasPickOne = choices.PickObject(ObjectType.Element);
                    Import.selElement = uidoc.Document.GetElement(hasPickOne.ElementId);
                }
                catch (Exception)
                {
                    MessageBox.Show("Необходимо выбрать графический вид!!!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return "";
                }
                
            }
            IList<ElementId> ImageExportList = new List<ElementId>();
            ImageExportList.Add(view.Id);
            var BilledeExportOptions = new ImageExportOptions
            {
                PixelSize = 512,
                ExportRange = ExportRange.SetOfViews,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_300,
                ZoomType = ZoomFitType.Zoom,
                ShadowViewsFileType = ImageFileType.PNG

            };

            BilledeExportOptions.ViewName = "";
          
            BilledeExportOptions.FilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Temp\" + view.Id.IntegerValue.ToString() + ".png";
            BilledeExportOptions.SetViewsAndSheets(ImageExportList);
            try
            {
                uidoc.Document.ExportImage(BilledeExportOptions);
            }
            catch { return ""; }
            DirectoryInfo imagesDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Temp\");
            string nameFile = "";

            nameFile = RenameFile(view, imagesDir, nameFile);
            //Image im = Image.FromFile(nameFile);
            //im = resizeImage(im, new Size(256, 256));
            //string newName = Path.GetFileNameWithoutExtension(nameFile);
            //newName += "f.png";
            //im.Save(newName);
            //using (var image = Image.FromFile(nameFile))
            //using (var newImage = ScaleImage(image, 300, 400))
            //{
            //    newImage.Save(@"c:\test.png", ImageFormat.Png);
            //}
            
            return nameFile;
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static string RenameFile(Autodesk.Revit.DB.View view, DirectoryInfo imagesDir, string nameFile)
        {
            foreach (FileInfo file in imagesDir.GetFiles())
            {
                if (!file.Name.Contains(view.Id.IntegerValue.ToString())) continue;
                var files = imagesDir.GetFiles().Where(x => x.FullName.Equals(Path.GetDirectoryName(file.FullName) + "\\" + view.Id.IntegerValue.ToString() + ".png")).ToList();
                if (files.Count > 0) files[0].Delete();
                file.MoveTo(Path.GetDirectoryName(file.FullName) + "\\" + view.Id.IntegerValue.ToString() + ".png");
                nameFile = (Path.GetDirectoryName(file.FullName) + "\\" + view.Id.IntegerValue.ToString() + ".png");
                break;

            }
            return nameFile;
        }

        public void ImportimageToProject(string pathToImage)
        {
            UIDocument uidoc = Import.appRevit.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Element element = null;
                using (Transaction transaction = new Transaction(doc, "Импорт изображения"))
                {
                    transaction.Start();
                    ImageImportOptions options = new ImageImportOptions();
                    options.Placement = Autodesk.Revit.DB.BoxPlacement.Center;
                    doc.Import(pathToImage, options, doc.ActiveView, out element);
                    string[] name = Path.GetFileName(pathToImage).Split('.');
                    doc.Delete(element.Id);
                    var rasterImages = new FilteredElementCollector(doc).OfClass(typeof(ImageType)).OfCategory(BuiltInCategory.OST_RasterImages).ToList().Where(x => x.Name.Contains(name[0])).ToList();
                    if (rasterImages.Count == 0) return;
                    ImageType im = rasterImages[0] as ImageType;
                   
                    Element el = doc.GetElement(Import.selElement.Id);
                    Parameter par = el.GetParameters("Изображение")[0];
                    par.Set(im.Id);
                    transaction.Commit();
                }
            }
            catch { }
        }


        public void ReloadImages()
        {
            ProcessManager pm = new ProcessManager();
            UIDocument uidoc = Reload.appRevit.ActiveUIDocument;
            Document doc = uidoc.Document;

            var rasterImages = new FilteredElementCollector(doc).OfClass(typeof(ImageType)).OfCategory(BuiltInCategory.OST_RasterImages).ToList();
            using (Transaction transaction = new Transaction(doc, "Обновление изображений"))
            {
                transaction.Start();
                foreach (ImageType imageElement in rasterImages)
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(imageElement.Name);
                        int viewId = 0;
                        bool isValid = Int32.TryParse(fileName, out viewId);
                        if (!isValid) continue;
                        ElementId elId = new ElementId(viewId);
                        Element elView = doc.GetElement(elId);
                        pm.GetImageFromView(uidoc, (Autodesk.Revit.DB.View)elView, true);
                        imageElement.Reload();
                    }
                    catch { }
                }
                transaction.Commit();
            }

        }
    }
}
